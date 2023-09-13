using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float zoomSpeed = 0.7f; // Adjust this to control zoom speed.

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = maxZoom;
    }

    // Update is called once per frame
    void Update()
    {
        float zoomInput = Input.mouseScrollDelta.y; // Use y-axis for zoom input
        float newSize = cam.orthographicSize - zoomInput * zoomSpeed;

        // Clamp the new size to the minZoom and maxZoom values
        newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

        // Set the camera's orthographic size to the new size
        cam.orthographicSize = newSize;
    }
}
