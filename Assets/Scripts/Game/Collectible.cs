using System;
using Unity.Netcode;
using UnityEngine;

public class Collectible : NetworkBehaviour
{
    private bool collected = false; // Flag to track if the collectible has been collected

    public static EventHandler OnCollectibleCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (!collected && other.CompareTag("Player"))
            {
                SoundManager.Instance.PlayCoinSound(this);
                GolfGameManager.Instance.PlayerCollectCollectible(OwnerClientId);
                collected = true; // Mark the collectible as collected to prevent multiple collections
                NetworkObject.Despawn();
            }
        }
        if (!collected && other.CompareTag("Player"))
        {
            SoundManager.Instance.PlayCoinSound(this);
            collected = true;
            PlayerCollectedCollectibleServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void PlayerCollectedCollectibleServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GolfGameManager.Instance.PlayerCollectCollectible(serverRpcParams.Receive.SenderClientId);
        collected = true; // Mark the collectible as collected to prevent multiple collections\
        NetworkObject.Despawn();
    }
}
