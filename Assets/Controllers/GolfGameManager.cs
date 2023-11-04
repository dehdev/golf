using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfGameManager : MonoBehaviour
{
    public static GolfGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResumed;

    private Vector3 spawnPoint;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    private bool isLocalPlayerReady;
    private float countdownToStartTimer = 3f;
    private float waitingToStartTimer = 1f;
    private float gamePlayingTimer;
    private float gameplayingTimerMax = 300f;
    private bool isGamePaused = false;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
        spawnPoint = GameObject.FindWithTag("SpawnPoint").transform.position;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnResetAction += GameInput_OnResetAction;
        PlayerController.spawnPlayer += PlayerController_spawnPlayer;
    }

    private void PlayerController_spawnPlayer(object sender, EventArgs e)
    {
        PlayerController.Instance.MoveToPos(spawnPoint);
        PlayerController.Instance.StopRbRotation();
    }

    private void GameInput_OnResetAction(object sender, EventArgs e)
    {
        if (!isGamePlaying())
        {
            return;
        }
        PlayerController.Instance.MoveToPos(spawnPoint);
        PlayerController.Instance.StopRbRotation();
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0f)
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer <= 0f)
                {
                    state = State.GamePlaying;
                    gamePlayingTimer = gameplayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    public bool isGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool isGameOver()
    {
        return state == State.GameOver;
    }

    public bool isCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public float GetGamePlayingTimerInSeconds()
    {
        return gamePlayingTimer;
    }

    public string GetGamePlayingTimer()
    {
        return GetGameTimer(gamePlayingTimer);
    }

    public string GetGamePlayingTimerMax()
    {
        return GetGameTimer(gameplayingTimerMax);
    }

    public string GetGameTimer(float timeInSeconds)
    {
        int totalSeconds = Mathf.RoundToInt(timeInSeconds);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameResumed?.Invoke(this, EventArgs.Empty);
        }
    }
}
