using Cinemachine;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float zoomSpeed = 0.7f; // Adjust this to control zoom speed.
    [SerializeField] private float zoomSmoothTime = 0.2f; // Adjust this for smoothing.

    private float targetZoom;
    private float currentZoomVelocity;

    private CinemachineVirtualCamera playerVirtualCamera;
    private CinemachineVirtualCamera finishedVirtualCamera;

    // Start is called before the first frame update
    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        FinishManager.Instance.OnLocalPlayerFinished += FinishManager_OnLocalPlayerFinished;
        //GameInput.Instance.OnStartedRotatingCamera += Instance_OnRotatingCamera;
        if (IsOwner)
        {
            finishedVirtualCamera = GameObject.FindGameObjectWithTag("FinishedCamera")?.GetComponent<CinemachineVirtualCamera>();
            playerVirtualCamera = GameObject.FindGameObjectWithTag("PlayerCamera")?.GetComponent<CinemachineVirtualCamera>();
            playerVirtualCamera.Follow = transform;
            playerVirtualCamera.m_Lens.OrthographicSize = maxZoom;
            targetZoom = maxZoom;
        }
    }

    private void FinishManager_OnLocalPlayerFinished(object sender, EventArgs e)
    {
        if (IsOwner)
        {
            SwitchCameraPriority();
        }
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (IsOwner && GolfGameManager.Instance.IsGameOver())
        {
            SwitchCameraPriority();
        }
    }

    private void SwitchCameraPriority()
    {
        if (!IsOwner)
        {
            return;
        }
        if (playerVirtualCamera.Priority == 1)
        {
            playerVirtualCamera.Priority = 0;
            finishedVirtualCamera.Priority = 1;
        }
    }

    private void Instance_OnRotatingCamera(object sender, EventArgs e)
    {
        Vector2 inputVector = (Vector2)sender;

        // Modify the localEulerAngles based on the inputVector
        Vector3 currentEulerAngles = playerVirtualCamera.transform.localEulerAngles;

        currentEulerAngles.y += inputVector.x; // Conver mouse x axis to camera y axis

        // Assign the modified angles back to the transform
        playerVirtualCamera.transform.localEulerAngles = currentEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (GolfGameManager.Instance.IsLocalPlayerPaused() || !GolfGameManager.Instance.IsGamePlaying() || GolfGameManager.Instance.IsLocalPlayerFinished() || !IsOwner)
        {
            return;
        }
        float zoomInput = Input.mouseScrollDelta.y; // Use y-axis for zoom input
        targetZoom -= zoomInput * zoomSpeed;

        // Clamp the target zoom to the minZoom and maxZoom values 
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Smoothly adjust the camera's orthographic size
        playerVirtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(playerVirtualCamera.m_Lens.OrthographicSize, targetZoom, ref currentZoomVelocity, zoomSmoothTime);
    }
}
