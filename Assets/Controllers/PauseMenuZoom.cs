using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuZoom : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;

    [SerializeField] private float optionsMenuZoom = 7f;
    [SerializeField] private float zoomSmoothTime = 0.2f;
    private bool isPaused = false;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(isPaused);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleMenu();
        }
    }

    public void HandleMenu()
    {
        isPaused = !isPaused;
        PauseMenu.SetActive(isPaused);
        cam.GetComponent<CameraZoom>().enabled = !isPaused;

        // Start a coroutine to smoothly adjust the orthographic size
        StartCoroutine(SmoothZoom(optionsMenuZoom));
    }

    private IEnumerator SmoothZoom(float targetSize)
    {
        float elapsedTime = 0f;
        float initialSize = cam.orthographicSize;

        while (elapsedTime < zoomSmoothTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / zoomSmoothTime);
            cam.orthographicSize = Mathf.Lerp(initialSize, targetSize, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = targetSize;
    }
}
