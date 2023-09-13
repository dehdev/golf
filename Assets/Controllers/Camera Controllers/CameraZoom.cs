using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float zoomSpeed = 0.7f; // Adjust this to control zoom speed.
    [SerializeField] private float zoomSmoothTime = 0.2f; // Adjust this for smoothing.

    private float targetZoom;
    private float currentZoomVelocity;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = maxZoom;
        targetZoom = maxZoom;
    }

    // Update is called once per frame
    void Update()
    {
        float zoomInput = Input.mouseScrollDelta.y; // Use y-axis for zoom input
        targetZoom -= zoomInput * zoomSpeed;

        // Clamp the target zoom to the minZoom and maxZoom values 
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Smoothly adjust the camera's orthographic size
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref currentZoomVelocity, zoomSmoothTime);
    }
}
