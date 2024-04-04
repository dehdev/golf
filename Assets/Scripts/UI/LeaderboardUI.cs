using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private Transform playerListTemplate;


    private void Start()
    {
        GameInput.Instance.OnToggleScoreboard += Instance_OnToggleScoreboard;
        GolfGameManager.Instance.OnConnectedClientsIdsReceived += GolfGameManager_OnConnectedClientsIdsReceived;
        GolfGameManager.Instance.OnPlayerShotDictionaryChanged += GolfGameManager_OnPlayerShotDictionaryChanged;
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        playerListTemplate.gameObject.SetActive(false);
        GolfGameManager.Instance.GetConnectedClientsIdsServerRpc();
        Hide();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsGameOver())
        {
            Hide();
        }
    }

    private void GolfGameManager_OnPlayerShotDictionaryChanged(object sender, EventArgs e)
    {
        if (gameObject.activeSelf)
        {
            UpdatePlayerList();
        }
    }

    private void GolfGameManager_OnConnectedClientsIdsReceived(object sender, EventArgs e)
    {
        UpdatePlayerList();
    }

    private void Instance_OnToggleScoreboard(object sender, EventArgs e)
    {
        if (gameObject.activeSelf)
        {
            Hide();
            return;
        }
        Show();
        GolfGameManager.Instance.GetConnectedClientsIdsServerRpc();
    }

    private void UpdatePlayerList()
    {
        // Create a list to store player IDs and their corresponding shot counts
        List<KeyValuePair<ulong, int>> playerShotCounts = new List<KeyValuePair<ulong, int>>();

        // Fill the list with player IDs and their shot counts
        foreach (ulong clientId in GolfGameManager.Instance.GetConnectedClientsIds())
        {
            int shotCount = GolfGameManager.Instance.GetPlayerShots(clientId);
            playerShotCounts.Add(new KeyValuePair<ulong, int>(clientId, shotCount));
        }

        playerShotCounts.OrderBy(pair => pair.Value);

        // Sort the list based on shot counts (ascending order)
        playerShotCounts.Sort((a, b) => a.Value.CompareTo(b.Value));

        // Clear the existing player list
        foreach (Transform child in playerListContainer)
        {
            if (child == playerListTemplate) continue;
            Destroy(child.gameObject);
        }

        // Instantiate and populate the player list based on the sorted list
        foreach (var pair in playerShotCounts)
        {
            Transform playerListTransform = Instantiate(playerListTemplate, playerListContainer);
            playerListTransform.gameObject.SetActive(true);
            playerListTransform.GetComponent<PlayerListSingleUI>().SetPlayerListSingleData(pair.Key, GolfGameMultiplayer.Instance.GetPlayerDataFromClientId(pair.Key).playerName.ToString(), pair.Value);
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnToggleScoreboard -= Instance_OnToggleScoreboard;
        GolfGameManager.Instance.OnConnectedClientsIdsReceived -= GolfGameManager_OnConnectedClientsIdsReceived;
        GolfGameManager.Instance.OnPlayerShotDictionaryChanged -= GolfGameManager_OnPlayerShotDictionaryChanged;
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
    }
}