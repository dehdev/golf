using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    public static GamePauseUI Instance { get; private set; }

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButon;
    [SerializeField] private Button quitGameButton;

    private void Awake()
    {
        Instance = this;
        resumeButton.onClick.AddListener(() =>
        {
            GolfGameManager.Instance.TogglePauseGame();
        });
        quitGameButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Application.Quit();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButon.onClick.AddListener(() =>
        {
            Hide();
            OptionsUI.Instance.Show();
        });
    }

    private void Start()
    {
        GolfGameManager.Instance.OnLocalGamePaused += GolfGameManager_OnLocalGamePaused;
        GolfGameManager.Instance.OnLocalGameUnpaused += GolfGameManager_OnLocalGameUnpaused;
        Hide();
    }

    private void GolfGameManager_OnLocalGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GolfGameManager_OnLocalGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
