using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GolfGameManager : NetworkBehaviour
{
    public static GolfGameManager Instance { get; private set; }

    private Dictionary<ulong, Boolean> playerReadyDictionary;

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResumed;
    public event EventHandler OnLocalPlayerReadyChanged;

    private int shots = 0;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private NetworkVariable<State> state = new(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new(3f);
    private NetworkVariable<float> gamePlayingTimer = new(0f);
    private float gameplayingTimerMax = 10f;
    private bool isGamePaused = false;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnResetAction += GameInput_OnResetAction;
        GameInput.Instance.OnAnyKeyPressed += GameInput_OnAnyKeyPressed;
        PlayerController.OnBallHit += PlayerController_OnBallHit;
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnAnyKeyPressed(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            SetPlayerReadyServerRpc();
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientdId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientdId) || !playerReadyDictionary[clientdId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
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

    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value <= 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gameplayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value <= 0f)
                {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public float GetGamePlayingTimerInSeconds()
    {
        return gamePlayingTimer.Value;
    }

    public string GetGamePlayingTimer()
    {
        return GetGameTimer(gamePlayingTimer.Value);
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
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
