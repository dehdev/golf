using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForInputUI : MonoBehaviour
{
    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        GolfGameManager.Instance.OnLocalPlayerReadyChanged += GolfGameManager_OnLocalPlayerReadyChanged;
        Show();
    }

    private void GolfGameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
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
