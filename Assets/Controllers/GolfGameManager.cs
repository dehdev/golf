using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GolfGameManager : MonoBehaviour
{
    public static GolfGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResumed;

    private int shots = 0;

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
    private float gamePlayingTimer;
    private float gameplayingTimerMax = 300f;
    private bool isGamePaused = false;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnResetAction += GameInput_OnResetAction;
        GameInput.Instance.OnAnyKeyPressed += GameInput_OnAnyKeyPressed;
        PlayerController.OnBallHit += PlayerController_OnBallHit;
    }

    private void GameInput_OnAnyKeyPressed(object sender, EventArgs e)
    {
        if (state == State.WaitingToStart)
        {
            state = state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayerController_OnBallHit(object sender, EventArgs e)
    {
        shots++;
    }

    public int GetShots()
    {
        return shots;
    }

    private void GameInput_OnResetAction(object sender, EventArgs e)
    {
        if (!IsGamePlaying())
        {
            return;
        }
        //PlayerController.Instance.MoveToPos(spawnPoint);
        //PlayerController.Instance.StopRbRotation();
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

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public bool IsCountdownToStartActive()
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
