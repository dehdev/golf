using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }

    [Header("Player Sounds")]
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip ballHit;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private AudioClip countdownSound;
    [SerializeField] private AudioClip finishSound;
    [SerializeField] private AudioClip offMapSound;
    [SerializeField] private AudioClip coinSound;

    public EventHandler OnSoundEffectsVolumeChanged;

    private float volume = 1f;

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

    void Start()
    {
        PlayerController.OnIdleEvent += PlayerController_OnIdleEvent;
        PlayerController.OnBallHit += PlayerController_OnBallHit;
        PlayerController.OnCollisionHit += PlayerController_OnCollisionHit;
        PlayerController.OnPlayerResetPosition += PlayerController_OnPlayerOffMap;
    }

    public void PlayCoinSound(object sender)
    {
        PlaySound(coinSound, (sender as Collectible).transform.position);
        Debug.Log("Coin sound played");
    }

    private void PlayerController_OnPlayerOffMap(object sender, EventArgs e)
    {
        PlayerController playerController = sender as PlayerController;
        PlaySound(offMapSound, playerController.transform.position);
    }

    public void PlayFinishedSound(object sender, EventArgs e)
    {
        FinishManager finishManager = sender as FinishManager;
        PlaySound(finishSound, finishManager.transform.position);
    }

    private void PlayerController_OnBallHit(object sender, EventArgs e)
    {
        PlayerController playerController = sender as PlayerController;
        PlaySound(ballHit, playerController.transform.position);
    }

    private void PlayerController_OnCollisionHit(object sender, float e)
    {
        PlayerController playerController = sender as PlayerController;
        PlaySound(collisionSound, playerController.transform.position, e);
    }

    private void PlayerController_OnIdleEvent(object sender, EventArgs e)
    {
        PlayerController playerController = sender as PlayerController;
        float isSilent = 1;
        if (!GolfGameManager.Instance.IsGamePlaying())
        {
            isSilent = 0;
        }
        PlaySound(idleSound, playerController.transform.position, 0.2f * isSilent);
    }

    public void PlayCountdownSound()
    {
        PlaySound(countdownSound, Vector3.zero, volume);
    }


    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        //AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
        PlayClipAt(audioClip, position, volumeMultiplier * volume);
    }

    public void IncreaseVolume()
    {
        if (volume < 1f)
        {
            volume += 0.1f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
        OnSoundEffectsVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DecreaseVolume()
    {
        if (volume >= 0.01f)
        {
            volume -= 0.1f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
        OnSoundEffectsVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetVolume()
    {
        return volume;
    }

    public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float volume)
    {
        GameObject tempGO = new("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        aSource.volume = volume;
        aSource.Play(); // start the sound
        Destroy(tempGO, clip.length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }

    private void OnDestroy()
    {
        PlayerController.OnIdleEvent -= PlayerController_OnIdleEvent;
        PlayerController.OnBallHit -= PlayerController_OnBallHit;
        PlayerController.OnCollisionHit -= PlayerController_OnCollisionHit;
        PlayerController.OnPlayerResetPosition -= PlayerController_OnPlayerOffMap;
    }
}
