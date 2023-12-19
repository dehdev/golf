using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListSingleUI : MonoBehaviour
{
    [SerializeField] private Image playerThumbnail;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerShotsText;

    public void SetPlayerListSingleData(ulong clientId, string playerName, int playerShots)
    {
        PlayerData playerData = GolfGameMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
        playerThumbnail.color = GolfGameMultiplayer.Instance.GetPlayerColor(playerData.colorId);
        playerNameText.text = playerName.ToUpper();
        playerShotsText.text = playerShots.ToString();
    }
}
