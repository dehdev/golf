using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FinishManager : NetworkBehaviour
{
    public static FinishManager Instance { get; private set; }

    public Dictionary<ulong, bool> playerFinishedDictionary;

    public event EventHandler OnLocalPlayerFinished;
    public event EventHandler OnMultiplayerGameFinished;

    private void Awake()
    {
        Instance = this;
        playerFinishedDictionary = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }
        base.OnNetworkSpawn();
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        playerFinishedDictionary.Remove(clientId);
        CheckAllPlayers();
    }

    private void Start()
    {
        if (IsServer)
        {
            playerFinishedDictionary.Add(NetworkManager.Singleton.LocalClientId, false);
            SetPlayerInFinishDictionaryClientRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            SetPlayerInFinishDictionaryServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerInFinishDictionaryServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerFinishedDictionary.Add(serverRpcParams.Receive.SenderClientId, false);
        SetPlayerInFinishDictionaryClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SetPlayerInFinishDictionaryClientRpc(ulong clientId)
    {
        if (IsServer)
        {
            return;
        }
        if (!playerFinishedDictionary.ContainsKey(clientId))
        {
            playerFinishedDictionary.Add(clientId, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnLocalPlayerFinished?.Invoke(this, EventArgs.Empty);
            PlayerController.LocalInstance.TogglePlayerFinishColliderServerRpc(false);
            SoundManager.Instance.PlayFinishedSound(this, EventArgs.Empty);
            SetPlayerFinishedServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerFinishedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        playerFinishedDictionary[clientId] = true;
        SetPlayerFinishedClientRpc(clientId);
        CheckAllPlayers();
    }

    [ClientRpc]
    private void SetPlayerFinishedClientRpc(ulong clientId)
    {
        if (IsServer)
        {
            return;
        }
        playerFinishedDictionary[clientId] = true;
        CheckAllPlayers();
    }

    private void CheckAllPlayers()
    {
        bool allPlayersFinished = true;
        foreach (ulong clientId in playerFinishedDictionary.Keys)
        {
            if (playerFinishedDictionary[clientId] == false)
            {
                allPlayersFinished = false;
            }
        }
        if (allPlayersFinished)
        {
            OnMultiplayerGameFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}