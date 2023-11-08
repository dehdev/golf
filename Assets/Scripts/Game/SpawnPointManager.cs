using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> m_SpawnPoints;
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
            playerSpawnPointDictionary.Add(clientId, m_SpawnPoints[clientsToAssign].transform.position);
            Debug.Log("Spawn point assigned to client: " + clientId + " at " + m_SpawnPoints[clientsToAssign].name);
            GetSpawnPointForClientId(clientId);
            clientsToAssign++;
        }
    }

    private void GetSpawnPointForClientId(ulong clientId)
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

    IEnumerator SpawnPlayer(ulong clientId)
    {
        Debug.Log("coroutine started");
        yield return new WaitForSeconds(.5f);
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>().SetSpawnPositionClientRpc(playerSpawnPointDictionary[clientId], clientId);
    }
}