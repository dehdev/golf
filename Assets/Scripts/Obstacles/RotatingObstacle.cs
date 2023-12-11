using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RotatingObstacle : NetworkBehaviour
{
    [SerializeField] private float rotationSpeed = 3f;

    private Tween rotateTween;

    private void Start()
    {
        if (IsServer)
        {
            RotateTween();
        }
    }

    private void RotateTween()
    {
        rotateTween = transform.DORotate(new Vector3(0f, 360f, 0f), rotationSpeed, RotateMode.FastBeyond360)
        .SetLoops(-1, LoopType.Restart)
        .SetRelative()
        .SetUpdate(UpdateType.Fixed)
        .SetEase(Ease.Linear);
    }

    public override void OnDestroy()
    {
        rotateTween.Kill();
    }
}