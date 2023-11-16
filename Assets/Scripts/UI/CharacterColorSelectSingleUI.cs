using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedImage;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GolfGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        GolfGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
        image.color = GolfGameMultiplayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void Instance_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (GolfGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedImage.SetActive(true);
        } else
        {
            selectedImage.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GolfGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= Instance_OnPlayerDataNetworkListChanged;
    }
}
