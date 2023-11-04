using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shotsText;

    private void Start()
    {
        PlayerController.OnBallHit += PlayerController_OnBallHit;
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        Hide();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.isGamePlaying())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void PlayerController_OnBallHit(object sender, EventArgs e)
    {
        shotsText.text = PlayerController.Instance.getShots().ToString();
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
