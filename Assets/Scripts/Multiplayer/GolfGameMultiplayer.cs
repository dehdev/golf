using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GolfGameMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;

    public static GolfGameMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "GAME HAS ALREADY STARTED";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= MAX_PLAYER_AMOUNT)
        {
            response.Approved = false;
            response.Reason = "GAME IS FULL";
            return;
        }
        response.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString())
        {
            return;
        }
        OnFailedToJoinGame.Invoke(this, EventArgs.Empty);
    }
}
