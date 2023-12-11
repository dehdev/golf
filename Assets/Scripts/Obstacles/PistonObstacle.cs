using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class PistonObstacle : NetworkBehaviour
{
    [SerializeField] private float moveDistance = 4f;
    [SerializeField] private float pullBackDuration = 2f;
    [SerializeField] private float pushDuration = 1f;

    private Tween pistonTween;

    private ObstacleBouncePlayer obstacleBouncePlayer;

    private void Start()
    {
        obstacleBouncePlayer = GetComponentInChildren<ObstacleBouncePlayer>();
        if (IsServer)
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
            .SetDelay(0.3f)
            .OnComplete(() =>
            {
                obstacleBouncePlayer.Enable();
                pistonTween = transform.DOLocalMoveY(initialPosition.y, pushDuration)
                    .SetEase(Ease.OutCubic)
                    .SetDelay(1f)
                    .OnComplete(() =>
                    {
                        MovePiston();
                    });
            });
    }

    public override void OnDestroy()
    {
        pistonTween.Kill();
    }
}
