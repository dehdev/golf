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
    [SerializeField] private Button mainMenuButton;
    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
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
