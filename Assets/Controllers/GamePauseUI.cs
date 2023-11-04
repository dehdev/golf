using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButon;
    [SerializeField] private Button quitGameButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GolfGameManager.Instance.TogglePauseGame();
        });
        quitGameButton.onClick.AddListener(() =>
        {
            Application.Quit();
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

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
