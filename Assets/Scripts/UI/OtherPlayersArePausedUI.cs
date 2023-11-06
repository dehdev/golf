using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayersArePausedUI : MonoBehaviour
{

    private void Start()
    {
        GolfGameManager.Instance.OnMultiplayerGamePaused += GolfGameManager_OnMultiplayerGamePaused;
        GolfGameManager.Instance.OnMultiplayerGameUnPaused += GolfGameManager_OnMultiplayerGameUnPaused;
        Hide();
    }

    private void GolfGameManager_OnMultiplayerGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GolfGameManager_OnMultiplayerGamePaused(object sender, EventArgs e)
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
