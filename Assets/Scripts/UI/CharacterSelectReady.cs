using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectReady : NetworkBehaviour
{

    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;

    public event EventHandler OnReadyStatusChanged;

    public NetworkVariable<Loader.GameScene> selectedGameScene;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        selectedGameScene.OnValueChanged += SelectedGameScene_OnValueChanged;
    }

    private void SelectedGameScene_OnValueChanged(Loader.GameScene previousValue, Loader.GameScene newValue)
    {
        selectedGameScene.Value = newValue;
    }

    public Loader.GameScene GetSelectedGameScene()
    {
        return selectedGameScene.Value;
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach (ulong clientdId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientdId) || !playerReadyDictionary[clientdId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            GolfGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(selectedGameScene.Value);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnReadyStatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
