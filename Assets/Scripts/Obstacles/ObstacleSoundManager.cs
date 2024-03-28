using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static ObstacleSoundManager Instance { get; private set; }

    [Header("Obstacle Sounds")]
    [SerializeField] private AudioClip targetSound;
    [SerializeField] private AudioClip wreckingBallSound;
    [SerializeField] private AudioClip pistonSound;
    [SerializeField] private AudioClip pistonReversedSound;
    [SerializeField] private AudioClip rotatingObstacleSound;

    private float volume = 1f;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
        }
    }

    private void Start()
    {
        SoundManager.Instance.OnSoundEffectsVolumeChanged += SoundManager_OnSoundEffectsVolumeChanged;
        TargetObstacle.OnTargetHit += Target_OnTargetHit;
        WreckingBallObstacle.OnWreckingBallStartMoving += WreckingBall_OnWreckingBallStartMoving;
        PistonObstacle.OnPistonObstacleStartMovingForward += PistonObstacle_OnPistonObstacleStartMovingForward;
        PistonObstacle.OnPistonObstacleStartMovingReverse += PistonObstacle_OnPistonObstacleStartMovingReverse;
        RotatingObstacle.OnRotatingObstacleStartRotating += RotatingObstacle_OnRotatingObstacleStartRotating;
        GolfGameManager.Instance.OnLocalGamePaused += GolfGameManager_OnLocalGamePaused;
        GolfGameManager.Instance.OnLocalGameUnpaused += GolfGameManager_OnLocalGameUnpaused;
    }

    private void GolfGameManager_OnLocalGameUnpaused(object sender, EventArgs e)
    {
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
        isPaused = false;
        SetObstacleAudioVolumes();
    }

    private void GolfGameManager_OnLocalGamePaused(object sender, EventArgs e)
    {
        volume = 0;
        SetObstacleAudioVolumes();
        isPaused = true;
    }

    private void PistonObstacle_OnPistonObstacleStartMovingForward(object sender, EventArgs e)
    {
        PistonObstacle pistonObstacle = sender as PistonObstacle;
        PlaySound(pistonSound, pistonObstacle.transform.position, false);
    }

    private void PistonObstacle_OnPistonObstacleStartMovingReverse(object sender, EventArgs e)
    {
        PistonObstacle pistonObstacle = sender as PistonObstacle;
        PlaySound(pistonReversedSound, pistonObstacle.transform.position, false);
    }

    private void RotatingObstacle_OnRotatingObstacleStartRotating(object sender, EventArgs e)
    {
        RotatingObstacle rotatingObstacle = sender as RotatingObstacle;
        PlaySound(rotatingObstacleSound, rotatingObstacle.transform.position, true);
    }

    private void WreckingBall_OnWreckingBallStartMoving(object sender, EventArgs e)
    {
        WreckingBallObstacle wreckingBallObstacle = sender as WreckingBallObstacle;
        PlaySound(wreckingBallSound, wreckingBallObstacle.transform.position, false);
    }

    private void SoundManager_OnSoundEffectsVolumeChanged(object sender, EventArgs e)
    {
        if (isPaused)
        {
            return;
        }
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
        SetObstacleAudioVolumes();
    }

    private void Target_OnTargetHit(object sender, EventArgs e)
    {
        TargetObstacle targetObstacle = sender as TargetObstacle;
        PlaySound(targetSound, targetObstacle.transform.position, false, spatialBlend: 0.5f);
    }

    void SetObstacleAudioVolumes()
    {
        // Find all GameObjects with the name "ObstacleAudio"
        GameObject[] obstacleAudioObjects = GameObject.FindGameObjectsWithTag("ObstacleAudio");

        // Iterate through each obstacle audio object
        foreach (GameObject obj in obstacleAudioObjects)
        {
            // Get the AudioSource component attached to the object
            AudioSource audioSource = obj.GetComponent<AudioSource>();

            // Check if AudioSource component exists
            if (audioSource != null)
            {
                // Set the volume of the AudioSource to 0.5
                audioSource.volume = volume;
            }
            else
            {
                Debug.LogWarning("No AudioSource component found on GameObject named 'ObstacleAudio'.");
            }
        }
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, bool loop, float volumeMultiplier = 1f, float spatialBlend = 1f)
    {
        //AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
        PlayClipAt(audioClip, position, volumeMultiplier * volume, loop, spatialBlend);
    }

    public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float volume, bool loop, float spatialBlend)
    {
        GameObject tempGO = new GameObject("ObstacleAudio"); // create the temp object
        tempGO.tag = "ObstacleAudio";
        tempGO.transform.position = pos; // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        aSource.volume = volume * 0.3f;
        aSource.loop = loop; // set loop property
        aSource.spatialBlend = spatialBlend; // set spatial sound to 3D
        aSource.rolloffMode = AudioRolloffMode.Custom; // set linear rolloff mode
        aSource.maxDistance = 35f; // set max distance
        aSource.Play(); // start the sound

        if (!loop)
        {
            Destroy(tempGO, clip.length); // destroy object after clip duration if not looped
        }

        return aSource; // return the AudioSource reference
    }


    private void OnDestroy()
    {
        TargetObstacle.OnTargetHit -= Target_OnTargetHit;
        WreckingBallObstacle.OnWreckingBallStartMoving -= WreckingBall_OnWreckingBallStartMoving;
        RotatingObstacle.OnRotatingObstacleStartRotating -= RotatingObstacle_OnRotatingObstacleStartRotating;
        SoundManager.Instance.OnSoundEffectsVolumeChanged -= SoundManager_OnSoundEffectsVolumeChanged;
        GolfGameManager.Instance.OnLocalGamePaused -= GolfGameManager_OnLocalGamePaused;
        GolfGameManager.Instance.OnLocalGameUnpaused -= GolfGameManager_OnLocalGameUnpaused;
    }
}
