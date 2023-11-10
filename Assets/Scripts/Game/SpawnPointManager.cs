using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> SpawnPoints;
    private Dictionary<ulong, Vector3> playerSpawnPointDictionary;

    public static SpawnPointManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerSpawnPointDictionary = new Dictionary<ulong, Vector3>();
    }

    private void Start()
    {
        if (!IsServer)
        {
            return;
        }

        int clientsToAssign = 0;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            playerSpawnPointDictionary.Add(clientId, SpawnPoints[clientsToAssign].transform.position);
            Debug.Log("Spawn point assigned to client: " + clientId + " at " + SpawnPoints[clientsToAssign].name);
            SetSpawnPointForClientId(clientId);
            clientsToAssign++;
        }
    }

    private void SetSpawnPointForClientId(ulong clientId)
    {
        if (playerSpawnPointDictionary.ContainsKey(clientId))
        {
            StartCoroutine(SpawnPlayer(clientId));
        }
        else
        {
            Debug.LogError("No spawn point found for client: " + clientId);
        }
    }

    public Vector3 GetSpawnPointForClientId(ulong clientId)
    {
        if (playerSpawnPointDictionary.ContainsKey(clientId))
        {
            return playerSpawnPointDictionary[clientId];
        }
        else
        {
            Debug.LogError("No spawn point found for client: " + clientId);
            return Vector3.zero;
        }
    }

    IEnumerator SpawnPlayer(ulong clientId)
    {
        yield return new WaitForSeconds(.5f);
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>().SetPlayerPositionClientRpc(playerSpawnPointDictionary[clientId]);
    }
}