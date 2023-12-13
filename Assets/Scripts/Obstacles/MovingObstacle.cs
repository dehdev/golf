using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private Vector3 moveDistance = new Vector3(0f, 0f, 0f);
    [SerializeField] private float moveDuration = 2f;

    private Tween platformMoveTween;

    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsCountdownToStartActive())
        {
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        Vector3 initialPosition = transform.position;

        platformMoveTween = transform.DOMove(initialPosition + moveDistance, moveDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() =>
            {
                platformMoveTween = transform.DOMove(initialPosition, moveDuration)
                    .SetEase(Ease.InOutQuad)
                    .SetUpdate(UpdateType.Fixed)
                    .OnComplete(() =>
                    {
                        MovePlatform();
                    });
            });
    }

    private void OnDestroy()
    {
        platformMoveTween.Kill();
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
    }
}
