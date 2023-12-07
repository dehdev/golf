using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBallObstacle : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private float rotationAngle = 45f;

    [SerializeField] private Transform wreckingBall1;
    [SerializeField] private Transform wreckingBall2;


    // Start is called before the first frame update
    void Start()
    {
        MoveWreckingBalls();
    }

    private void MoveWreckingBalls()
    {
        MoveWreckingBallOnX(wreckingBall1, rotationAngle);
        MoveWreckingBallOnY(wreckingBall2, -rotationAngle);
    }

    private void MoveWreckingBallOnX(Transform wreckingBall, float targetRotation)
    {
        wreckingBall.DORotate(new Vector3(targetRotation, 0, 0), rotationDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => MoveWreckingBallOnX(wreckingBall, -targetRotation));
    }

    private void MoveWreckingBallOnY(Transform wreckingBall, float targetRotation)
    {
        wreckingBall.DORotate(new Vector3(0, 90, targetRotation), rotationDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => MoveWreckingBallOnY(wreckingBall, -targetRotation));
    }

    private void OnDestroy()
    {
        // Kill any ongoing tweens associated with this GameObject
        DOTween.Kill(wreckingBall1);
        DOTween.Kill(wreckingBall2);
    }
}
