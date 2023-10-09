using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsScript : MonoBehaviour
{
    [SerializeField] GameObject checkBox;
    [SerializeField] TextMeshPro resolutionText;
    [SerializeField] TextMeshPro qualityText;
    [SerializeField] GameObject applyButton;

    Resolution[] resolutions;
    List<Resolution> filteredResolutions;
    int currentResolutionIndex = 0;
    int currentQualityIndex = 0;

    public GameObject[] GeneralVolumeTiles, MusicVolumeTiles;
    private Renderer[] GeneralVolumeTileRenderers, MusicVolumeTileRenderers;
    public AudioMixer GeneralAudioMixer, MusicAudioMixer;
    int currentGeneralVolumeIndex = -1;
    int currentMusicVolumeIndex = -1;

    private Camera mainCamera;

    bool isFullscreen = true;

    Renderer checkBoxRenderer;

    public Material selected, notSelected;

    private void Start()
    {
        currentGeneralVolumeIndex = PlayerPrefs.GetInt("GeneralVolume", 2);
        currentMusicVolumeIndex = PlayerPrefs.GetInt("MusicVolume", 2);
        SetVolume();

        GeneralVolumeTileRenderers = new Renderer[10];
        for (int i = 0; i < 3; i++)
        {
            GeneralVolumeTileRenderers[i] = GeneralVolumeTiles[i].GetComponent<Renderer>();
            if (i <= currentGeneralVolumeIndex)
            {
                GeneralVolumeTileRenderers[i].material = selected;
            }
            else
            {
                GeneralVolumeTileRenderers[i].material = notSelected;
            }
        }

        MusicVolumeTileRenderers = new Renderer[10];
        for (int i = 0; i < 3; i++)
        {
            MusicVolumeTileRenderers[i] = MusicVolumeTiles[i].GetComponent<Renderer>();
            if (i <= currentMusicVolumeIndex)
            {
                MusicVolumeTileRenderers[i].material = selected;
            }
            else
            {
                MusicVolumeTileRenderers[i].material = notSelected;
            }
        }

        resolutions = Screen.resolutions;

        // Create a list to store 16:9 resolutions.
        filteredResolutions = new List<Resolution>();

        // Iterate through all resolutions and filter 16:9 resolutions.
        foreach (Resolution res in resolutions)
        {
            // Check if the aspect ratio is 16:9 (approximately 1.7778).
            if (Mathf.Approximately((float)res.width / res.height, 16f / 9f))
            {
                // Add the resolution to the filtered list.
                filteredResolutions.Add(res);
            }
        }
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        mainCamera = Camera.main;
        checkBoxRenderer = checkBox.GetComponent<Renderer>();
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            isFullscreen = false;
            checkBoxRenderer.material = notSelected;
        }
        else
        {
            isFullscreen = true;
            checkBoxRenderer.material = selected;
        }
        resolutionText.text = filteredResolutions[currentResolutionIndex].width + "x" + filteredResolutions[currentResolutionIndex].height;
        currentQualityIndex = QualitySettings.GetQualityLevel();
        qualityText.text = QualitySettings.names[currentQualityIndex];

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the ray hits a GameObject
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject != null)
                {
                    if (clickedObject.name == "Cycle res -")
                    {
                        applyButton.SetActive(true);
                        if (currentResolutionIndex > 0)
                        {
                            currentResolutionIndex--;
                            resolutionText.text = filteredResolutions[currentResolutionIndex].width + "x" + filteredResolutions[currentResolutionIndex].height;
                        }
                    }
                    else if (clickedObject.name == "Cycle res +")
                    {
                        applyButton.SetActive(true);
                        if (currentResolutionIndex < filteredResolutions.Count - 1)
                        {
                            currentResolutionIndex++;
                            resolutionText.text = filteredResolutions[currentResolutionIndex].width + "x" + filteredResolutions[currentResolutionIndex].height;
                        }
                    }
                    else if (clickedObject.name == "Fullscreen checkbox")
                    {
                        if (isFullscreen)
                        {
                            Screen.fullScreen = false;
                            isFullscreen = false;
                            checkBoxRenderer.material = notSelected;
                        }
                        else
                        {
                            Screen.fullScreen = true;
                            isFullscreen = true;
                            checkBoxRenderer.material = selected;
                        }
                    }
                    else if (clickedObject.name == "Cycle quality -")
                    {
                        if (currentQualityIndex > 0)
                        {
                            applyButton.SetActive(true);
                            currentQualityIndex--;
                            qualityText.text = QualitySettings.names[currentQualityIndex];
                        }
                    }
                    else if (clickedObject.name == "Cycle quality +")
                    {
                        if (currentQualityIndex < 5)
                        {
                            applyButton.SetActive(true);
                            currentQualityIndex++;
                            qualityText.text = QualitySettings.names[currentQualityIndex];
                        }
                    }
                    else if (clickedObject.name == "GVolume +")
                    {
                        if (currentGeneralVolumeIndex < 2)
                        {
                            applyButton.SetActive(true);
                            currentGeneralVolumeIndex++;
                            GeneralVolumeTileRenderers[currentGeneralVolumeIndex].material = selected;
                            SetVolume();
                        }
                    }
                    else if (clickedObject.name == "GVolume -")
                    {
                        if (currentGeneralVolumeIndex >= 0)
                        {
                            if (currentGeneralVolumeIndex == 0)
                            {
                                applyButton.SetActive(true);
                                GeneralVolumeTileRenderers[0].material = notSelected;
                            }
                            else
                            {
                                applyButton.SetActive(true);
                                GeneralVolumeTileRenderers[currentGeneralVolumeIndex].material = notSelected;
                            }
                            currentGeneralVolumeIndex--;
                            SetVolume();
                        }
                    }
                    else if (clickedObject.name == "MVolume +")
                    {
                        if (currentMusicVolumeIndex < 2)
                        {
                            applyButton.SetActive(true);
                            currentMusicVolumeIndex++;
                            MusicVolumeTileRenderers[currentMusicVolumeIndex].material = selected;
                            SetVolume();
                        }
                    }
                    else if (clickedObject.name == "MVolume -")
                    {
                        if (currentMusicVolumeIndex >= 0)
                        {
                            if (currentMusicVolumeIndex == 0)
                            {
                                applyButton.SetActive(true);
                                MusicVolumeTileRenderers[0].material = notSelected;
                            }
                            else
                            {
                                applyButton.SetActive(true);
                                MusicVolumeTileRenderers[currentMusicVolumeIndex].material = notSelected;
                            }
                            currentMusicVolumeIndex--;
                            SetVolume();
                        }
                    }
                    else if (clickedObject.name == "Apply text")
                    {
                        Screen.SetResolution(filteredResolutions[currentResolutionIndex].width, filteredResolutions[currentResolutionIndex].height, isFullscreen);
                        QualitySettings.SetQualityLevel(currentQualityIndex);
                        SetVolume();
                        PlayerPrefs.SetInt("GeneralVolume", currentGeneralVolumeIndex);
                        PlayerPrefs.SetInt("MusicVolume", currentMusicVolumeIndex);
                        PlayerPrefs.Save();
                        applyButton.SetActive(false);
                    }
                }
            }
        }
    }
    public void SetVolume()
    {
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
