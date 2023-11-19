using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaineMenuOptionsUI : MonoBehaviour
{
    public static MaineMenuOptionsUI Instance { get; private set; }

    [SerializeField] private Button backButton;
    [SerializeField] private Button soundEffectsVolumePlus;
    [SerializeField] private Button soundEffectsVolumeMinus;
    [SerializeField] private Button musicVolumePlus;
    [SerializeField] private Button musicVolumeMinus;

    [SerializeField] private TextMeshProUGUI soundEffectsVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

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
        backButton.onClick.AddListener(() =>
        {
            MainMenuUI.Instance.Show();
            Hide();
        });
    }

    private void Start()
    {
        UpdateVisual();
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsVolumeText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicVolumeText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
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
