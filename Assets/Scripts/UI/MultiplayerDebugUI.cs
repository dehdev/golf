using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerDebugUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            GolfGameMultiplayer.Instance.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() =>
        {
            GolfGameMultiplayer.Instance.StartClient();
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
