using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shotsText;
    [SerializeField] private TextMeshProUGUI finishLabel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Image finishLabelImage;

    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        FinishManager.OnLocalPlayerFinished += Instance_OnLocalPlayerFinished;
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        Hide();
    }

    private void Instance_OnLocalPlayerFinished(object sender, EventArgs e)
    {
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
        shotsText.text = PlayerController.LocalInstance.GetLocalPlayerShots().ToString();
        finishLabel.text = "FINISHED IN";
        finishLabelImage.color = new Color(0.1529412f, 0.682353f, 0.3764706f, 1f);
        Show();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsGameOver())
        {
            FinishManager.OnLocalPlayerFinished -= Instance_OnLocalPlayerFinished;
            shotsText.text = PlayerController.LocalInstance.GetLocalPlayerShots().ToString();
            finishLabel.text = "DIDN'T FINISH IN";
            Show();
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
