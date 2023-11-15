using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] GameObject readyText;
    [SerializeField] PlayerVisual playerVisual;
    private void Start()
    {
        GolfGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyStatusChanged += Instance_OnReadyStatusChanged;
        UpdatePlayer();
    }

    private void Instance_OnReadyStatusChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void Instance_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (GolfGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = GolfGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyText.SetActive(CharacterSelectReady.Instance.isPlayerReady(playerData.clientId));
            playerVisual.SetPlayerColor(GolfGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
