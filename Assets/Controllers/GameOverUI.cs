using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shotsText;

    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        Hide();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsGameOver())
        {
            Show();
            shotsText.text = GolfGameManager.Instance.GetShots().ToString();
        }
        else
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
