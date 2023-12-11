using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovingObstacle : NetworkBehaviour
{
    [SerializeField] private Vector3 moveDistance = new Vector3(0f, 0f, 0f);
    [SerializeField] private float moveDuration = 2f;

    private Tween platformMoveTween;

    private void Start()
    {
        if (IsServer)
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

    public override void OnDestroy()
    {
        platformMoveTween.Kill();
    }
}
