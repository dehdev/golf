using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { get; private set; }

    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiPlayerButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image titleTextBackground;

    private void Awake()
    {
        Instance = this;
        singlePlayerButton.onClick.AddListener(() =>
        {
            GolfGameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        multiPlayerButton.onClick.AddListener(() =>
        {
            GolfGameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        optionsButton.onClick.AddListener(() =>
        {
            MaineMenuOptionsUI.Instance.Show();
            Hide();
        });
        Time.timeScale = 1;
        Cursor.visible = true;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
