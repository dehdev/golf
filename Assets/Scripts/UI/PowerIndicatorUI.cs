using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIndicatorUI : MonoBehaviour
{
    [SerializeField] private RectTransform powerIndicatorMaskRectTransform;

    private void Start()
    {
        PlayerController.OnShootingStartEvent += PlayerController_OnShootingStartEvent;
        PlayerController.OnBallHit += PlayerController_OnBallHit;
        PlayerController.OnPlayerResetPosition += PlayerController_OnPlayerResetPosition;
        GameInput.Instance.OnCancelShoot += GameInput_OnCancelShoot;
        powerIndicatorMaskRectTransform.sizeDelta = new Vector2(0, 0);
    }

    private void GameInput_OnCancelShoot(object sender, EventArgs e)
    {
        ResetIndicator();
    }

    private void PlayerController_OnPlayerResetPosition(object sender, EventArgs e)
    {
        ResetIndicator();
    }

    private void PlayerController_OnBallHit(object sender, EventArgs e)
    {
        ResetIndicator();
    }

    private void PlayerController_OnShootingStartEvent(object sender, float power)
    {
        powerIndicatorMaskRectTransform.sizeDelta = new Vector2(256 * power, 256 * power);
    }

    private void ResetIndicator()
    {
        powerIndicatorMaskRectTransform.sizeDelta = new Vector2(0, 0);
    }

    private void OnDestroy()
    {
        PlayerController.OnShootingStartEvent -= PlayerController_OnShootingStartEvent;
        PlayerController.OnBallHit -= PlayerController_OnBallHit;
        PlayerController.OnPlayerResetPosition -= PlayerController_OnPlayerResetPosition;
        GameInput.Instance.OnCancelShoot -= GameInput_OnCancelShoot;
    }
}
