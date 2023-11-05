using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class ClientServerSpawn : NetworkBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient;
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        if (NetworkManager.gameObject.TryGetComponent<SpawnPointManager>(out var spawnPointManager))
        {
            rb.position = spawnPointManager.GetSpawnPoint();
        }
        var virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
    }
}
