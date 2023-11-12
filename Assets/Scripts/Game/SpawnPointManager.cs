using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> SpawnPoints;
    private Dictionary<ulong, Vector3> playerSpawnPointDictionary;
    private int spawnPointIndex;

    public static SpawnPointManager Instance { get; private set; }
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerSpawnPointDictionary = new Dictionary<ulong, Vector3>();
        spawnPointIndex = SpawnPoints.Count;
        spawnPointIndex--;
    }

    private void Start()
    {
        if (!IsServer)
        {
            return;
        }
        GolfGameManager.Instance.OnLocalPlayerSpawned += GolfGameManager_OnLocalPlayerSpawned;
        IsInitialized = true;
    }

    private void GolfGameManager_OnLocalPlayerSpawned(object sender, EventArgs e)
    {
        ulong clientId = (ulong)sender;
        playerSpawnPointDictionary.Add(clientId, SpawnPoints[spawnPointIndex].transform.position);
        Debug.Log("Spawn point assigned to client: " + clientId + " at " + SpawnPoints[spawnPointIndex].name);
        SetSpawnPointForClientId(clientId);
        spawnPointIndex--;
    }

    private void SetSpawnPointForClientId(ulong clientId)
    {
        if (playerSpawnPointDictionary.ContainsKey(clientId))
        {
            //StartCoroutine(SpawnPlayer(clientId));
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>().SetPlayerPositionClientRpc(playerSpawnPointDictionary[clientId]);

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
        yield return new WaitForSeconds(0.1f);
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>().SetPlayerPositionClientRpc(playerSpawnPointDictionary[clientId]);
    }
}