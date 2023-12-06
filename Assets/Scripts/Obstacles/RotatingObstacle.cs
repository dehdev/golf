using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.1f;

    private Tween rotateTween;

    private void Start()
    {
        RotateTween();
    }

    private void RotateTween()
    {
        rotateTween = transform.DORotate(new Vector3(0f, 360f, 0f), rotationSpeed, RotateMode.FastBeyond360)
        .SetLoops(-1, LoopType.Restart)
        .SetRelative()
        .SetEase(Ease.Linear);
    }
}
