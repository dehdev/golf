using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UISoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    private float volume = 1f;
    private AudioSource audioSource;

    private void Awake()
    {
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
        SoundManager.Instance.OnSoundEffectsVolumeChanged += SoundManager_OnSoundEffectsVolumeChanged;
    }

    private void SoundManager_OnSoundEffectsVolumeChanged(object sender, EventArgs e)
    {
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
        audioSource.volume = volume;
    }

    public void PlaySound()
    {
        audioSource.Play();
    }

    private void OnDestroy()
    {
        SoundManager.Instance.OnSoundEffectsVolumeChanged -= SoundManager_OnSoundEffectsVolumeChanged;
    }
}
