using System;
using System.Collections;
using System.Collections.Generic;
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
            Application.Quit();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
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
        GolfGameManager.Instance.OnGamePaused += GolfGameManager_OnGamePaused;
        GolfGameManager.Instance.OnGameResumed += GolfGameManager_OnGameResumed;
        Hide();
    }

    private void GolfGameManager_OnGameResumed(object sender, EventArgs e)
    {
        Hide();
    }

    private void GolfGameManager_OnGamePaused(object sender, EventArgs e)
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
