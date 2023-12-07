using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private Vector3 moveDistance = new Vector3(0f, 0f, 0f);
    [SerializeField] private float moveDuration = 2f;

    private Tween platformMoveTween;

    private void Start()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        Vector3 initialPosition = transform.position;

        platformMoveTween = transform.DOMove(initialPosition + moveDistance, moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                platformMoveTween = transform.DOMove(initialPosition, moveDuration)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        MovePlatform();
                    });
            });
    }

    private void OnDestroy()
    {
        platformMoveTween.Kill();
    }
}
