using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GolfGameManager : NetworkBehaviour
{
    public static GolfGameManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;

    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private Dictionary<ulong, int> playerShotsDictionary;

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnPaused;
    public event EventHandler OnLocalPlayerSpawned;

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
    private float gameplayingTimerMax = 30f;
    private bool isLocalGamePaused = false;
    private bool autoTestGamePausedState = false;
    private NetworkVariable<bool> isGamePaused = new(false);

    private bool didLocalPlayerFinish = false;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
        playerShotsDictionary = new Dictionary<ulong, int>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnAnyKeyPressed += GameInput_OnAnyKeyPressed;
        FinishManager.Instance.OnLocalPlayerFinished += FinishManager_OnLocalPlayerFinished;
        FinishManager.Instance.OnMultiplayerGameFinished += FinishManager_OnMultiplayerGameFinished;
    }

    private void FinishManager_OnLocalPlayerFinished(object sender, EventArgs e)
    {
        didLocalPlayerFinish = true;
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
        base.OnNetworkSpawn();
    }

    private void FinishManager_OnMultiplayerGameFinished(object sender, EventArgs e)
    {
        state.Value = State.GameOver;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer)
        {
            return;
        }
        foreach (ulong clientId in clientsCompleted)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            StartCoroutine(WaitForSpawnPointManager(clientId));
        }
    }

    IEnumerator WaitForSpawnPointManager(ulong clientId)
    {
        while (!SpawnPointManager.Instance.IsInitialized)
        {
            yield return null;
        }
        OnLocalPlayerSpawned?.Invoke(clientId, EventArgs.Empty);
        playerShotsDictionary.Add(clientId, 0);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
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
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerBallHitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (!IsClient)
        {
            return;
        }
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            playerShotsDictionary[serverRpcParams.Receive.SenderClientId]++;
        }
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

    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            autoTestGamePausedState = false;
            TestGamePausedState();
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
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnPauseGameServerRpc();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }

    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }

    public bool IsLocalPlayerPaused()
    {
        return isLocalGamePaused;
    }

    public bool IsLocalPlayerFinished()
    {
        return didLocalPlayerFinish;
    }
}
