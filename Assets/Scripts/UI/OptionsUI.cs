using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button backButton;
    [SerializeField] private Button soundEffectsVolumePlus;
    [SerializeField] private Button soundEffectsVolumeMinus;
    [SerializeField] private Button musicVolumePlus;
    [SerializeField] private Button musicVolumeMinus;
    [SerializeField] private Button ambientVolumePlus;
    [SerializeField] private Button ambientVolumeMinus;

    [SerializeField] private TextMeshProUGUI soundEffectsVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI ambientVolumeText;

    private void Awake()
    {
        Instance = this;
        soundEffectsVolumeMinus.onClick.AddListener(() =>
        {
            SoundManager.Instance.DecreaseVolume();
            UpdateVisual();
        });
        soundEffectsVolumePlus.onClick.AddListener(() =>
        {
            SoundManager.Instance.IncreaseVolume();
            UpdateVisual();
        });
        musicVolumeMinus.onClick.AddListener(() =>
        {
            MusicManager.Instance.DecreaseVolume();
            UpdateVisual();
        });
        musicVolumePlus.onClick.AddListener(() =>
        {
            MusicManager.Instance.IncreaseVolume();
            UpdateVisual();
        });
        ambientVolumeMinus.onClick.AddListener(() =>
        {
            AmbientManager.Instance.DecreaseVolume();
            UpdateVisual();
        });
        ambientVolumePlus.onClick.AddListener(() =>
        {
            AmbientManager.Instance.IncreaseVolume();
            UpdateVisual();
        });
        backButton.onClick.AddListener(() =>
        {
            Hide();
            GamePauseUI.Instance.Show();
        });
    }

    private void Start()
    {
        GolfGameManager.Instance.OnLocalGameUnpaused += GolfGameManager_OnGameResumed;
        UpdateVisual();
        Hide();
    }

    private void GolfGameManager_OnGameResumed(object sender, EventArgs e)
    {
        Hide();
        GamePauseUI.Instance.Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsVolumeText.text = "EFFECTS: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicVolumeText.text = "MUSIC: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
        ambientVolumeText.text = "AMBIENT: " + Mathf.Round(AmbientManager.Instance.GetVolume() * 10f);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
