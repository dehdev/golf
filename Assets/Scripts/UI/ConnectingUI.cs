using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        GolfGameMultiplayer.Instance.OnTryingToJoinGame += GolfGameMultiplayer_OnTryingToJoinGame;
        GolfGameMultiplayer.Instance.OnFailedToJoinGame += GolfGameMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void OnDestroy()
    {
        GolfGameMultiplayer.Instance.OnTryingToJoinGame -= GolfGameMultiplayer_OnTryingToJoinGame;
        GolfGameMultiplayer.Instance.OnFailedToJoinGame -= GolfGameMultiplayer_OnFailedToJoinGame;
    }

    private void GolfGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void GolfGameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
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
