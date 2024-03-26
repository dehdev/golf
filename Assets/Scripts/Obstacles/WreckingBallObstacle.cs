using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WreckingBallObstacle : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private float rotationAngle = 45f;

    [SerializeField] private Transform wreckingBall1;
    [SerializeField] private Transform wreckingBall2;

    public static EventHandler OnWreckingBallStartMoving;

    // Start is called before the first frame update
    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsCountdownToStartActive())
        {
            MoveWreckingBalls();
        }
    }

    private void MoveWreckingBalls()
    {
        MoveWreckingBallOnX(wreckingBall1, rotationAngle);
        MoveWreckingBallOnY(wreckingBall2, -rotationAngle);
    }

    private void MoveWreckingBallOnX(Transform wreckingBall, float targetRotation)
    {
        wreckingBall.DOLocalRotate(new Vector3(targetRotation, 0, 0), rotationDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(UpdateType.Fixed)
            .OnStart(() => OnWreckingBallStartMoving?.Invoke(this, EventArgs.Empty))
            .OnComplete(() => MoveWreckingBallOnX(wreckingBall, -targetRotation));
    }

    private void MoveWreckingBallOnY(Transform wreckingBall, float targetRotation)
    {
        wreckingBall.DOLocalRotate(new Vector3(0, 90, targetRotation), rotationDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() => MoveWreckingBallOnY(wreckingBall, -targetRotation));
    }

    private void OnDestroy()
    {
        // Kill any ongoing tweens associated with this GameObject
        DOTween.Kill(wreckingBall1);
        DOTween.Kill(wreckingBall2);
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
    }
}
