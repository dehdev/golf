using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    private void Start()
    {
        GolfGameManager.Instance.OnLocalPlayerReadyChanged += GolfGameManager_OnLocalPlayerReadyChanged;
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        Hide();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void GolfGameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsLocalPlayerReady())
        {
            Show();
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
