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
    [SerializeField] private AudioClip rotatingObstacleSound;

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
       TargetObstacle.OnTargetHit += Target_OnTargetHit;
        //WreckingBallObstacle.OnWreckingBallHit += WreckingBall_OnWreckingBallHit;
        //PistonObstacle.OnPistonHit += Piston_OnPistonHit;
       // RotatingObstacle.OnRotatingObstacleHit += RotatingObstacle_OnRotatingObstacleHit;
    }

    private void WreckingBall_OnWreckingBallHit(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Target_OnTargetHit(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}
