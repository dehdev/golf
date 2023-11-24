using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AmbientManager : MonoBehaviour
{
    private const string PLAYER_PREFS_AMBIENT_VOLUME = "AmbientVolume";

    [SerializeField] private AudioClip windAmbient;

    public static AmbientManager Instance { get; private set; }

    private float volume = .1f;

    private AudioSource audioSource;

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
            volume = PlayerPrefs.GetFloat(PLAYER_PREFS_AMBIENT_VOLUME, .1f);
        }
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }

    public void IncreaseVolume()
    {
        if (volume < 1f)
        {
            volume += 0.1f;
        }
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_AMBIENT_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void DecreaseVolume()
    {
        if (volume >= 0.01f)
        {
            volume -= 0.1f;
        }
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_AMBIENT_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }

    public void PlayWindAmbient()
    {
        audioSource.clip = windAmbient;
        audioSource.Play();
        Debug.Log("Playing wind ambient");
    }

    public void StopAmbient()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}
