using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForInputUI : MonoBehaviour
{
    private void Start()
    {
        Show();
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
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
