using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float zoomSpeed = 0.7f; // Adjust this to control zoom speed.
    [SerializeField] private float zoomSmoothTime = 0.2f; // Adjust this for smoothing.

    private float targetZoom;
    private float currentZoomVelocity;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.OrthographicSize = maxZoom;
        targetZoom = maxZoom;
        //GameInput.Instance.OnStartedRotatingCamera += Instance_OnRotatingCamera;
    }

    private void Instance_OnRotatingCamera(object sender, EventArgs e)
    {
        Vector2 inputVector = (Vector2)sender;

        // Modify the localEulerAngles based on the inputVector
        Vector3 currentEulerAngles = virtualCamera.transform.localEulerAngles;

        currentEulerAngles.y += inputVector.x; // Conver mouse x axis to camera y axis

        // Assign the modified angles back to the transform
        virtualCamera.transform.localEulerAngles = currentEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (GolfGameManager.Instance.IsLocalPlayerPaused() || !GolfGameManager.Instance.IsGamePlaying() || GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        float zoomInput = Input.mouseScrollDelta.y; // Use y-axis for zoom input
        targetZoom -= zoomInput * zoomSpeed;

        // Clamp the target zoom to the minZoom and maxZoom values 
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Smoothly adjust the camera's orthographic size
        virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, targetZoom, ref currentZoomVelocity, zoomSmoothTime);
    }
}
