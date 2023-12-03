using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MaineMenuOptionsUI : MonoBehaviour
{
    public static MaineMenuOptionsUI Instance { get; private set; }

    private const string PLAYER_PREFS_RESOLUTION = "GameResolution";
    private const string PLAYER_PREFS_VIDEO_QUALITY = "GameQuality";
    private const string PLAYER_PREFS_VSYNC = "GameVsync";
    private const string PLAYER_PREFS_FULLSCREEN = "GameFullscreen";


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

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private RenderPipelineAsset[] qualityLevels;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex = 0;

    public void SetQualityLevelDropdown(int index)
    {
        QualitySettings.SetQualityLevel(index, false);
        QualitySettings.renderPipeline = qualityLevels[index];
        PlayerPrefs.SetInt(PLAYER_PREFS_VIDEO_QUALITY, index);
        PlayerPrefs.Save();
    }

    public void ApplyResolution(int index)
    {
        Resolution resolution = filteredResolutions[index];
        RefreshRate cRefreshRate = Screen.currentResolution.refreshRateRatio;
        Screen.SetResolution(resolution.width, resolution.height, GetFullScreenMode(), cRefreshRate);
        PlayerPrefs.SetInt(PLAYER_PREFS_RESOLUTION, index);
        PlayerPrefs.Save();
    }

    public void FullscreenToggle(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            PlayerPrefs.SetInt(PLAYER_PREFS_FULLSCREEN, 1);
            PlayerPrefs.Save();
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            PlayerPrefs.SetInt(PLAYER_PREFS_FULLSCREEN, 0);
            PlayerPrefs.Save();
        }
    }

    private FullScreenMode GetFullScreenMode()
    {
        if (PlayerPrefs.GetInt(PLAYER_PREFS_FULLSCREEN, 1) == 1)
        {
            return FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            return FullScreenMode.Windowed;
        }
    }

    public void VsyncToggle(bool isVsync)
    {
        if (isVsync)
        {
            QualitySettings.vSyncCount = 1;
            PlayerPrefs.SetInt(PLAYER_PREFS_VSYNC, 1);
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            PlayerPrefs.SetInt(PLAYER_PREFS_VSYNC, 0);
        }
        PlayerPrefs.Save();
    }

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
            MainMenuUI.Instance.Show();
            Hide();
        });
    }

    private void InitializeVideoSettings()
    {
        if (PlayerPrefs.GetInt(PLAYER_PREFS_FULLSCREEN, 1) == 1)
        {
            fullscreenToggle.isOn = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            fullscreenToggle.isOn = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        if (PlayerPrefs.GetInt(PLAYER_PREFS_VSYNC, 1) == 1)
        {
            vsyncToggle.isOn = true;
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            vsyncToggle.isOn = false;
            QualitySettings.vSyncCount = 0;
        }

        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        double targetRefreshRateRatio = Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            double currentRefreshRateRatio = resolutions[i].refreshRateRatio.value;

            // Check if the absolute difference is less than or equal to 1
            if (Mathf.Abs((float)currentRefreshRateRatio - (float)targetRefreshRateRatio) <= 1f)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions = filteredResolutions.Distinct().ToList();

        List<string> resolutionOptions = new();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            resolutionOptions.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    private void Start()
    {
        InitializeVideoSettings();
        UpdateVisual();
        Hide();
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
