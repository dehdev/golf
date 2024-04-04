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
                OnCollectibleCollected?.Invoke(this, EventArgs.Empty);
                collected = true; // Mark the collectible as collected to prevent multiple collections
                Debug.Log("Collectible collected");
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
        OnCollectibleCollected?.Invoke(this, EventArgs.Empty);
        collected = true; // Mark the collectible as collected to prevent multiple collections\
        Debug.Log("Collectible collected");
        NetworkObject.Despawn();
    }
}
