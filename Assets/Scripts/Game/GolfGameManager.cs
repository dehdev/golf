using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GolfGameManager : NetworkBehaviour
{
    public static GolfGameManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;

    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    public Dictionary<ulong, int> playerShotsDictionary;
    public Dictionary<ulong, int> playerRestartingDictionary;

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnPaused;

    public event EventHandler OnConnectedClientsIdsReceived;
    public event EventHandler OnPlayerShotDictionaryChanged;
    public event EventHandler onPlayerRestartingDictionaryChanged;

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
    [SerializeField] private float gameplayingTimerMax = 120f;
    private bool isLocalGamePaused = false;
    private bool autoTestGamePausedState = false;
    private bool autoTestGameReadyState = false;
    private NetworkVariable<bool> isGamePaused = new(false);

    private List<ulong> connectedClientsIds;

    private bool didLocalPlayerFinish = false;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
        playerShotsDictionary = new Dictionary<ulong, int>();
        playerRestartingDictionary = new Dictionary<ulong, int>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnAnyKeyPressed += GameInput_OnAnyKeyPressed;
        FinishManager.Instance.OnLocalPlayerFinished += FinishManager_OnLocalPlayerFinished;
        FinishManager.Instance.OnMultiplayerGameFinished += FinishManager_OnMultiplayerGameFinished;

        if (!IsServer)
        {
            return;
        }
        Collectible.OnCollectibleCollected += Collectible_OnCollectibleCollected;
    }

    private void Collectible_OnCollectibleCollected(object sender, EventArgs e)
    {
        Debug.Log("DO SOMETHING");
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetConnectedClientsIdsServerRpc()
    {
        GetConnectedClientsIdsClientRpc(NetworkManager.Singleton.ConnectedClientsIds.ToArray());
    }

    [ClientRpc]
    private void GetConnectedClientsIdsClientRpc(ulong[] list)
    {
        connectedClientsIds = new List<ulong>(list);
        OnConnectedClientsIdsReceived?.Invoke(this, EventArgs.Empty);
    }

    public List<ulong> GetConnectedClientsIds()
    {
        return connectedClientsIds;
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
            NetworkManager.Singleton.OnClientDisconnectCallback += (ulong clientId) => NetworkManager_OnClientDisconnectCallback(clientId);
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
        base.OnNetworkSpawn();
    }

    private void FinishManager_OnMultiplayerGameFinished(object sender, EventArgs e)
    {
        if (!IsServer)
        {
            return;
        }
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
        }
    }

    public void CleanScene()
    {
        List<NetworkObject> objectsToCleanUp = new List<NetworkObject>();

        foreach (NetworkObject networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            if (networkObject.IsPlayerObject && networkObject.gameObject.CompareTag("Player"))
            {
                objectsToCleanUp.Add(networkObject);
            }
        }

        foreach (NetworkObject networkObject in objectsToCleanUp)
        {
            networkObject.Despawn(true);
            Destroy(networkObject.gameObject);
        }
        NetworkManager.Singleton.OnClientDisconnectCallback -= (ulong clientId) => NetworkManager_OnClientDisconnectCallback(clientId);
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        // Null check before removing items from dictionaries
        if (playerReadyDictionary != null)
            playerReadyDictionary.Remove(clientId);

        if (playerPausedDictionary != null)
            playerPausedDictionary.Remove(clientId);

        if (playerShotsDictionary != null)
            playerShotsDictionary.Remove(clientId);

        if (playerRestartingDictionary != null)
            playerRestartingDictionary.Remove(clientId);

        // Update other states or variables as needed
        autoTestGamePausedState = true;
        autoTestGameReadyState = true;
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
            GameInput.Instance.DisableWaitingForInputMap();
            GameInput.Instance.OnAnyKeyPressed -= GameInput_OnAnyKeyPressed;
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

        if (allClientsReady && state.Value == State.WaitingToStart)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void CheckPlayersReady()
    {
        bool allClientsReady = true;
        foreach (ulong clientdId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientdId) || !playerReadyDictionary[clientdId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady && state.Value == State.WaitingToStart)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void CheckPlayersRestarting()
    {
        if (!IsServer)
        {
            return;
        }
        bool allPlayersRestarting = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerRestartingDictionary.ContainsKey(clientId) || playerRestartingDictionary[clientId] == 0)
            {
                allPlayersRestarting = false;
                break;
            }
        }
        if (allPlayersRestarting)
        {
            CleanScene();
            Loader.LoadNetwork(Loader.GetCurrentSceneEnum(SceneManager.GetActiveScene().name));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerBallHitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (!playerShotsDictionary.ContainsKey(clientId))
        {
            playerShotsDictionary.Add(clientId, 0);
        }
        playerShotsDictionary[clientId]++;
        OnPlayerShotDictionaryChanged?.Invoke(this, EventArgs.Empty);
        SetPlayerBallHitClientRpc(clientId);
    }

    [ClientRpc]
    private void SetPlayerBallHitClientRpc(ulong clientId)
    {
        if (IsServer)
        {
            return;
        }
        if (!playerShotsDictionary.ContainsKey(clientId))
        {
            playerShotsDictionary.Add(clientId, 0);
        }
        playerShotsDictionary[clientId]++;
        OnPlayerShotDictionaryChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerRestartingServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (!playerRestartingDictionary.ContainsKey(clientId))
        {
            playerRestartingDictionary.Add(clientId, 0);
        }
        playerRestartingDictionary[clientId]++;
        onPlayerRestartingDictionaryChanged?.Invoke(this, EventArgs.Empty);
        SetPlayerRestartingClientRpc(clientId);
        CheckPlayersRestarting();
    }

    [ClientRpc]
    private void SetPlayerRestartingClientRpc(ulong clientId)
    {
        if (IsServer)
        {
            return;
        }
        if (!playerRestartingDictionary.ContainsKey(clientId))
        {
            playerRestartingDictionary.Add(clientId, 0);
        }
        playerRestartingDictionary[clientId]++;
        onPlayerRestartingDictionaryChanged?.Invoke(this, EventArgs.Empty);
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
                PlayerController.LocalInstance.CancelShoot();
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
        if (autoTestGameReadyState)
        {
            autoTestGameReadyState = false;
            CheckPlayersReady();
        }
    }

    public int GetPlayerShots(ulong clientId)
    {
        if (playerShotsDictionary.ContainsKey(clientId))
        {
            return playerShotsDictionary[clientId];
        }
        return 0;
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
            PlayerController.LocalInstance.CancelShoot();
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
