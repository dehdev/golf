using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FinishManager : NetworkBehaviour
{
    public static FinishManager Instance { get; private set; }

    private Dictionary<ulong, bool> playerFinishedDictionary;

    public event EventHandler OnLocalPlayerFinished;
    public event EventHandler OnMultiplayerGameFinished;

    private void Awake()
    {
        Instance = this;
        playerFinishedDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        if (!IsServer)
        {
            return;
        }
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            playerFinishedDictionary.Add(clientId, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnLocalPlayerFinished?.Invoke(this, EventArgs.Empty);
            SetPlayerFinishServerRpc();
            SoundManager.Instance.PlayFinishedSound(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerFinishServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerFinishedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        CheckAllPlayers();
    }

    private void CheckAllPlayers()
    {
        bool allPlayersFinished = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
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
