using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayingTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private string timerTextValue;
    private bool isFlashing = false;
    private Color targetColor = Color.white;
    private float colorChangeSpeed = 3.0f; // Adjust the speed as needed

    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
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

    private void Update()
    {
        timerTextValue = GolfGameManager.Instance.GetGamePlayingTimer();

        if (GolfGameManager.Instance.GetGamePlayingTimerInSeconds() <= 10f)
        {
            if (!isFlashing)
            {
                StartCoroutine(FlashText());
            }
        }
        else
        {
            isFlashing = false;
            targetColor = Color.white;
        }

        timerText.text = timerTextValue;
        timerText.color = Color.Lerp(timerText.color, targetColor, Time.deltaTime * colorChangeSpeed);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private IEnumerator FlashText()
    {
        isFlashing = true;

        while (GolfGameManager.Instance.GetGamePlayingTimerInSeconds() <= 10f)
        {
            targetColor = Color.red;
            yield return new WaitForSeconds(1.0f); // Stay red for 1 second

            targetColor = Color.white;
            yield return new WaitForSeconds(1.0f); // Stay white for 1 second
        }

        isFlashing = false;
    }
}
