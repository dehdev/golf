using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerIdText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerShotsText;

    public void SetPlayerListSingleData(ulong clientId, string playerName, int playerShots)
    {
        playerIdText.text = clientId.ToString();
        playerNameText.text = playerName.ToUpper();
        playerShotsText.text = playerShots.ToString();
    }
}
