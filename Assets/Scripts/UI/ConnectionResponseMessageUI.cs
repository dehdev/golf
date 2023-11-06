using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


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
        Hide();
    }

    private void OnDestroy()
    {
        GolfGameMultiplayer.Instance.OnFailedToJoinGame -= GolfGameMultiplayer_OnFailedToJoinGame;
    }

    private void GolfGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();
        messageText.text = NetworkManager.Singleton.DisconnectReason;
        if (messageText.text == "")
        {
            messageText.text = "Failed to join game";
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
}
