using Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class ServerPlayerSpawn : NetworkBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        if (NetworkManager.gameObject.TryGetComponent<SpawnPointManager>(out var spawnPointManager))
        {
            rb.position = spawnPointManager.GetSpawnPoint();
            Debug.Log("HOST SPAWNED" + rb.position);
        }
        base.OnNetworkSpawn();
    }
}
