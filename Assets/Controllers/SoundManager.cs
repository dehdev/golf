using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip idleSound, ballHit, collisionSound, countdownSound;
    private float volume = 1f;

    private void Awake()
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    void Start()
    {
        PlayerController.OnIdleEvent += PlayerController_OnIdleEvent;
        PlayerController.OnBallHit += PlayerController_OnBallHit;
        PlayerController.OnCollisionHit += PlayerController_OnCollisionHit;
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
        PlaySound(idleSound, playerController.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

    public void PlayCountdownSound()
    {
        PlaySound(countdownSound, Vector3.zero, volume);
    }

    public void IncreaseVolume()
    {
        if (volume < 1f)
        {
            volume += 0.1f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void DecreaseVolume()
    {
        if (volume >= 0.1f)
        {
            volume -= 0.1f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
