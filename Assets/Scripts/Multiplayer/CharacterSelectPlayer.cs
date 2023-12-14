using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] GameObject readyText;
    [SerializeField] PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshProUGUI playerNameText;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = GolfGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            GolfGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            GolfGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        GolfGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += GolfGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyStatusChanged += CharacterSelectReady_OnReadyStatusChanged;
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerIndex != 0);
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyStatusChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void GolfGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (GolfGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = GolfGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerVisual.SetPlayerColor(GolfGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
            playerNameText.text = playerData.playerName.ToString();
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

    private void OnDestroy()
    {
        GolfGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= GolfGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
