using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shotsText;
    [SerializeField] private TextMeshProUGUI finishLabel;
    [SerializeField] private TextMeshProUGUI playersRestartCountLabel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button backToLobbiesButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image finishLabelImage;

    private void Awake()
    {
        backToLobbiesButton.gameObject.SetActive(false);
        restartButton.interactable = false;
    }

    private void Start()
    {
        FinishManager.Instance.OnLocalPlayerFinished += FinishManager_OnLocalPlayerFinished;
        FinishManager.Instance.OnMultiplayerGameFinished += FinishManager_OnMultiplayerGameFinished;
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        GolfGameManager.Instance.onPlayerRestartingDictionaryChanged += GolfGameManager_onPlayerRestartingDictionaryChanged;
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        restartButton.onClick.AddListener(() =>
        {
            GolfGameManager.Instance.SetPlayerRestartingServerRpc();
            restartButton.interactable = false;
        });

        if (GolfGameMultiplayer.playMultiplayer)
        {
            backToLobbiesButton.gameObject.SetActive(true);
            backToLobbiesButton.onClick.AddListener(() =>
            {
                GolfGameMultiplayer.Instance.ShutDownNetwork();
                Loader.Load(Loader.Scene.LobbyScene);
            });
        }
        Hide();
    }

    private void GolfGameManager_onPlayerRestartingDictionaryChanged(object sender, EventArgs e)
    {
        UpdatePlayersRestartingLabel();
    }

    private void FinishManager_OnMultiplayerGameFinished(object sender, EventArgs e)
    {
        restartButton.interactable = true;
        UpdatePlayersRestartingLabel();
    }

    private void UpdatePlayersRestartingLabel()
    {
        playersRestartCountLabel.text = GolfGameManager.Instance.playerRestartingDictionary.Count.ToString() + "/" + FinishManager.Instance.playerFinishedDictionary.Count;
    }

    private void FinishManager_OnLocalPlayerFinished(object sender, EventArgs e)
    {
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
        shotsText.text = PlayerController.LocalInstance.GetLocalPlayerShots().ToString();
        finishLabel.text = "FINISHED IN";
        finishLabelImage.color = new Color(0.1529412f, 0.682353f, 0.3764706f, 1f);
        Show();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsGameOver())
        {
            shotsText.text = PlayerController.LocalInstance.GetLocalPlayerShots().ToString();
            finishLabel.text = "DIDN'T FINISH IN";
            FinishManager.Instance.OnLocalPlayerFinished -= FinishManager_OnLocalPlayerFinished;
            Show();
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

    private void OnDestroy()
    {
        FinishManager.Instance.OnLocalPlayerFinished -= FinishManager_OnLocalPlayerFinished;
        FinishManager.Instance.OnMultiplayerGameFinished -= FinishManager_OnMultiplayerGameFinished;
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
    }
}
