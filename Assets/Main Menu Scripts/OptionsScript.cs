using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsScript : MonoBehaviour
{
    [SerializeField] GameObject checkBox;
    [SerializeField] TextMeshPro resolutionText;
    Resolution[] resolutions;
    List<Resolution> filteredResolutions;
    int currentResolutionIndex = 0;

    private Camera mainCamera;

    bool isFullscreen = true;

    Renderer checkBoxRenderer;

    public Material selected, notSelected;

    private void Start()
    {
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
        resolutionText.text = Screen.width + "x" + Screen.height;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the ray hits a GameObject
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject != null)
                {
                    if (clickedObject.name == "Cycle res -")
                    {
                        if (currentResolutionIndex > 0)
                        {
                            currentResolutionIndex--;
                            resolutionText.text = filteredResolutions[currentResolutionIndex].width + "x" + filteredResolutions[currentResolutionIndex].height;
                        }
                    }
                    else if (clickedObject.name == "Cycle res +")
                    {
                        if (currentResolutionIndex < resolutions.Length - 1)
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
                    else if (clickedObject.name == "Apply text")
                    {
                        Screen.SetResolution(filteredResolutions[currentResolutionIndex].width, filteredResolutions[currentResolutionIndex].height, isFullscreen);
                    }
                }
            }
        }
    }
}
