using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private Button navigateRight;
    [SerializeField] private Button navigateLeft;
    [SerializeField] private TextMeshProUGUI selectedLevelText;

    private Loader.GameScene currentEnumValue;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            navigateLeft.gameObject.SetActive(false);
            navigateRight.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        CharacterSelectReady.Instance.selectedGameScene.OnValueChanged += SelectedGameScene_OnValueChanged;
        RefreshValue();
    }

    private void SelectedGameScene_OnValueChanged(Loader.GameScene previousValue, Loader.GameScene newValue)
    {
        RefreshValue();
    }

    public void NavigateRightEnum()
    {
        int enumCount = System.Enum.GetValues(typeof(Loader.GameScene)).Length;

        int nextEnumValue = ((int)currentEnumValue + 1) % enumCount;
        currentEnumValue = (Loader.GameScene)nextEnumValue;

        selectedLevelText.text = currentEnumValue.ToString();

        CharacterSelectReady.Instance.selectedGameScene.Value = currentEnumValue;
    }

    public void NavigateLeftEnum()
    {
        int enumCount = System.Enum.GetValues(typeof(Loader.GameScene)).Length;

        int nextEnumValue = ((int)currentEnumValue - 1 + enumCount) % enumCount;
        currentEnumValue = (Loader.GameScene)nextEnumValue;

        selectedLevelText.text = currentEnumValue.ToString();

        CharacterSelectReady.Instance.selectedGameScene.Value = currentEnumValue;
    }

    public void RefreshValue()
    {
        selectedLevelText.text = CharacterSelectReady.Instance.selectedGameScene.Value.ToString();
    }
}
