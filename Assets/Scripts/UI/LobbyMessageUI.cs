using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    public int GolfGameLobby_OnJoinedLobby { get; private set; }

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        GolfGameMultiplayer.Instance.OnFailedToJoinGame += GolfGameMultiplayer_OnFailedToJoinGame;
        GolfGameLobby.Instance.OnCreateLobbyStarted += GolfGameLobby_OnCreateLobbyStarted;
        GolfGameLobby.Instance.OnCreateLobbyFailed += GolfGameLobby_OnCreateLobbyFailed;
        GolfGameLobby.Instance.OnJoinStarted += GolfGameLobby_OnJoinStarted;
        GolfGameLobby.Instance.OnJoinFailed += GolfGameLobby_OnJoinFailed;
        GolfGameLobby.Instance.OnQuickJoinFailed += GolfGameLobby_OnQuickJoinFailed;
        Hide();
    }

    private void GolfGameLobby_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("NO LOBBIES AVAILABLE!");
    }

    private void GolfGameLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("FAILED TO JOIN LOBBY!");
    }

    private void GolfGameLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("JOINING LOBBY...");
    }

    private void GolfGameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("CREATING LOBBY...");
    }

    private void GolfGameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("FAILED TO CREATE LOBBY!");
    }

    private void OnDestroy()
    {
        GolfGameMultiplayer.Instance.OnFailedToJoinGame -= GolfGameMultiplayer_OnFailedToJoinGame;
        GolfGameLobby.Instance.OnCreateLobbyStarted -= GolfGameLobby_OnCreateLobbyStarted;
        GolfGameLobby.Instance.OnCreateLobbyFailed -= GolfGameLobby_OnCreateLobbyFailed;
        GolfGameLobby.Instance.OnJoinStarted -= GolfGameLobby_OnJoinStarted;
        GolfGameLobby.Instance.OnJoinFailed -= GolfGameLobby_OnJoinFailed;
        GolfGameLobby.Instance.OnQuickJoinFailed -= GolfGameLobby_OnQuickJoinFailed;
    }

    private void GolfGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("FAILED TO CONNECT!");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
