using DG.Tweening;
using System;
using Unity.Netcode;
using UnityEngine;

public class PistonObstacle : MonoBehaviour
{
    [SerializeField] private float moveDistance = 4f;
    [SerializeField] private float pullBackDuration = 2f;
    [SerializeField] private float pushDuration = 1f;

    private Tween pistonTween;

    private ObstacleBouncePlayer obstacleBouncePlayer;

    public static EventHandler OnPistonObstacleStartMovingForward;
    public static EventHandler OnPistonObstacleStartMovingReverse;

    private void Start()
    {
        GolfGameManager.Instance.OnStateChanged += GolfGameManager_OnStateChanged;
        obstacleBouncePlayer = GetComponentInChildren<ObstacleBouncePlayer>();
    }

    private void GolfGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GolfGameManager.Instance.IsCountdownToStartActive())
        {
            MovePiston();
        }
    }

    private void MovePiston()
    {
        Vector3 initialPosition = transform.localPosition;
        obstacleBouncePlayer.Disable();
        pistonTween = transform.DOLocalMoveY(initialPosition.y - moveDistance, pullBackDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(UpdateType.Fixed)
            .SetDelay(0.3f)
            .OnStart(() => OnPistonObstacleStartMovingReverse?.Invoke(this, EventArgs.Empty))
            .OnComplete(() =>
            {
                obstacleBouncePlayer.Enable();
                pistonTween = transform.DOLocalMoveY(initialPosition.y, pushDuration)
                    .SetEase(Ease.OutCubic)
                    .SetUpdate(UpdateType.Fixed)
                    .SetDelay(1f)
                    .OnStart(() => OnPistonObstacleStartMovingForward?.Invoke(this, EventArgs.Empty))
                    .OnComplete(() =>
                    {
                        MovePiston();
                    });
            });
    }

    private void OnDestroy()
    {
        pistonTween.Kill();
        GolfGameManager.Instance.OnStateChanged -= GolfGameManager_OnStateChanged;
    }
}
