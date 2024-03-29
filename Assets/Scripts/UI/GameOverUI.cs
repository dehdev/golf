using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shotsText;
    [SerializeField] private TextMeshProUGUI finishLabel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button backToLobbiesButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image finishLabelImage;

    private void Awake()
    {
        backToLobbiesButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        FinishManager.Instance.OnLocalPlayerFinished += Instance_OnLocalPlayerFinished;
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        if (GolfGameMultiplayer.Instance.IsServer)
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.AddListener(() =>
            {
                Loader.LoadNetwork(Loader.GameScene.TUTORIAL);
            });
        }
        else
        {
            restartButton.gameObject.SetActive(false);
        }

        if (GolfGameMultiplayer.playMultiplayer)
        {
            backToLobbiesButton.gameObject.SetActive(true);
            backToLobbiesButton.onClick.AddListener(() =>
            {
                GolfGameMultiplayer.Instance.ShutDownNetwork();
                Loader.Load(Loader.Scene.LobbyScene);
            });
        }
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
            shotsText.text = PlayerController.LocalInstance.GetLocalPlayerShots().ToString();
            finishLabel.text = "DIDN'T FINISH IN";
            FinishManager.Instance.OnLocalPlayerFinished -= Instance_OnLocalPlayerFinished;
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
