using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class InitializeSettingsScript : MonoBehaviour
{
    public AudioMixer GeneralAudioMixer, MusicAudioMixer;

    void Start()
    {
        int currentGeneralVolumeIndex = PlayerPrefs.GetInt("GeneralVolume", 2);
        int currentMusicVolumeIndex = PlayerPrefs.GetInt("MusicVolume", 2);

        float minVolume = -80f; // Lowest volume (mute)
        float maxVolume = 0f;   // Maximum volume
        var generalVolume = currentGeneralVolumeIndex switch
        {
            -1 => minVolume,
            0 => -15f,
            1 => -10f,
            2 => maxVolume,
            _ => maxVolume,
        };
        var musicVolume = currentMusicVolumeIndex switch
        {
            -1 => minVolume,
            0 => -15f,
            1 => -10f,
            2 => maxVolume,
            _ => maxVolume,
        };
        GeneralAudioMixer.SetFloat("GeneralVolume", generalVolume);
        MusicAudioMixer.SetFloat("MusicVolume", musicVolume);
    }
}
