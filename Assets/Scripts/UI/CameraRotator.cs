using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;

    private Tween rotateCameraTween;
    private void Start()
    {
        RotateCameraTween();
    }

    private void RotateCameraTween()
    {
        rotateCameraTween = transform.DORotate(new Vector3(0f, 360f, 0f), rotationSpeed, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetRelative()
                    .SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        StopRotating();
    }

    public void StopRotating()
    {
        if (rotateCameraTween == null)
        {
            return;
        }
        rotateCameraTween.Kill();
    }
}
