using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shotsText;

    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        FinishManager.Instance.OnLocalPlayerFinished += Instance_OnLocalPlayerFinished;
        GolfGameManager.Instance.OnPlayerShotDictionaryChanged += GolfGameManager_OnPlayerShotDictionaryChanged;
        Hide();
    }

    private void GolfGameManager_OnPlayerShotDictionaryChanged(object sender, EventArgs e)
    {
        shotsText.text = GolfGameManager.Instance.GetPlayerShots(NetworkManager.Singleton.LocalClientId).ToString();
    }

    private void Instance_OnLocalPlayerFinished(object sender, EventArgs e)
    {
        Hide();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsGamePlaying())
        {
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
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
        FinishManager.Instance.OnLocalPlayerFinished -= Instance_OnLocalPlayerFinished;
    }
}