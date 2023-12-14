using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
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
        if (IsServer)
        {
            playerFinishedDictionary.Add(NetworkManager.Singleton.LocalClientId, false);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ulong clientId = other.gameObject.GetComponent<PlayerController>().OwnerClientId;
            SetLocalPlayerFinishedClientRpc(clientId);
        }
    }

    [ClientRpc]
    private void SetLocalPlayerFinishedClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            OnLocalPlayerFinished?.Invoke(this, EventArgs.Empty);
            PlayerController.LocalInstance.TogglePlayerFinishColliderServerRpc(false);
            SoundManager.Instance.PlayFinishedSound(this, EventArgs.Empty);
            SetPlayerFinishServerRpc(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerFinishServerRpc(ulong clientId)
    {
        playerFinishedDictionary[clientId] = true;
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
