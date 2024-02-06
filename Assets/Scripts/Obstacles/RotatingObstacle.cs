using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 3f;

    private Tween rotateTween;

    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsCountdownToStartActive())
        {
            RotateTween();
        }
    }

    private void RotateTween()
    {
        rotateTween = transform.DOLocalRotate(new Vector3(0f, 360f, 0f), rotationSpeed, RotateMode.FastBeyond360)
        .SetLoops(-1, LoopType.Restart)
        .SetRelative()
        .SetUpdate(UpdateType.Fixed)
        .SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        rotateTween.Kill();
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
    }
}