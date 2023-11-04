using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
        
    public static GameInput Instance { get; private set; }

    public event EventHandler OnPauseAction;
    public event EventHandler OnResetAction;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Pause.performed += Pause_performed;
        playerInputActions.Player.Resetplayerposition.performed += Resetplayerposition_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Pause.performed -= Pause_performed;
        playerInputActions.Player.Resetplayerposition.performed -= Resetplayerposition_performed;

        playerInputActions.Dispose();
    }

    private void Resetplayerposition_performed(InputAction.CallbackContext context)
    {
        OnResetAction?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }
}
