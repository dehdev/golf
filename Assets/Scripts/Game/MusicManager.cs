using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static MusicManager Instance { get; private set; }

    private float volume = .1f;
    private AudioSource audioSource;

    public AudioClip[] musicPlaylist;

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
            volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .1f);
        }
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }

    private void Start()
    {
        StartCoroutine(PlayRandomMusic());
    }

    private IEnumerator PlayRandomMusic()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, musicPlaylist.Length);
            audioSource.clip = musicPlaylist[randomIndex];
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length);
        }
    }

    public void IncreaseVolume()
    {
        if (volume < 1f)
        {
            volume += 0.1f;
        }
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void DecreaseVolume()
    {
        if (volume >= 0.01f)
        {
            volume -= 0.1f;
        }
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
