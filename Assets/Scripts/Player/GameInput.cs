using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    public static GameInput Instance { get; private set; }

    public event EventHandler OnPauseAction;
    public event EventHandler OnResetAction;
    public event EventHandler OnAnyKeyPressed;
    public event EventHandler OnCancelShoot;
    public event EventHandler OnStartedRotatingCamera;
    public event EventHandler OnToggleScoreboard;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.WaitingForInput.Enable();
        playerInputActions.RotateCamera.Disable();

        playerInputActions.Player.Pause.performed += Pause_performed;
        playerInputActions.Player.Resetplayerposition.performed += Resetplayerposition_performed;
        playerInputActions.Player.CancelShoot.performed += CancelShoot_performed;
        playerInputActions.Player.ActivateCameraRotate.performed += ActivateCameraRotate_performed;
        playerInputActions.Player.ActivateCameraRotate.canceled += ActivateCameraRotate_canceled;


        playerInputActions.Player.ToggleScoreboard.performed += ToggleScoreboard_performed;

        playerInputActions.RotateCamera.RotatingCamera.performed += RotatingCamera_performed;

        playerInputActions.WaitingForInput.Anykeypressed.performed += Anykeypressed_performed;
    }

    private void ToggleScoreboard_performed(InputAction.CallbackContext context)
    {
        if (GolfGameManager.Instance.IsLocalPlayerPaused() || !GolfGameManager.Instance.IsGamePlaying() || GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        OnToggleScoreboard?.Invoke(this, EventArgs.Empty);
    }

    private void RotatingCamera_performed(InputAction.CallbackContext context)
    {
        if (GolfGameManager.Instance.IsLocalPlayerPaused() || !GolfGameManager.Instance.IsGamePlaying() || GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        OnStartedRotatingCamera?.Invoke(context.ReadValue<Vector2>(), EventArgs.Empty);
    }

    private void ActivateCameraRotate_canceled(InputAction.CallbackContext context)
    {
        playerInputActions.RotateCamera.Disable();
    }

    private void ActivateCameraRotate_performed(InputAction.CallbackContext context)
    {
        if (GolfGameManager.Instance.IsLocalPlayerPaused() || !GolfGameManager.Instance.IsGamePlaying() || GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        playerInputActions.RotateCamera.Enable();
    }

    private void Anykeypressed_performed(InputAction.CallbackContext context)
    {
        if (!GolfGameManager.Instance.IsWaitingToStart())
        {
            return;
        }
        OnAnyKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Pause.performed -= Pause_performed;
        playerInputActions.Player.Resetplayerposition.performed -= Resetplayerposition_performed;
        playerInputActions.Player.CancelShoot.performed -= CancelShoot_performed;
        playerInputActions.WaitingForInput.Anykeypressed.performed -= Anykeypressed_performed;
        playerInputActions.Player.ActivateCameraRotate.performed -= ActivateCameraRotate_performed;
        playerInputActions.Player.ActivateCameraRotate.canceled -= ActivateCameraRotate_canceled;

        playerInputActions.Dispose();
    }

    private void CancelShoot_performed(InputAction.CallbackContext context)
    {
        if (GolfGameManager.Instance.IsLocalPlayerPaused() || !GolfGameManager.Instance.IsGamePlaying() || GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        OnCancelShoot?.Invoke(this, EventArgs.Empty);
    }

    private void Resetplayerposition_performed(InputAction.CallbackContext context)
    {
        if (GolfGameManager.Instance.IsLocalPlayerPaused() || !GolfGameManager.Instance.IsGamePlaying() || GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        OnResetAction?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(InputAction.CallbackContext context)
    {
        if (GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    public void DisableWaitingForInputMap()
    {
        playerInputActions.WaitingForInput.Disable();
    }
}
