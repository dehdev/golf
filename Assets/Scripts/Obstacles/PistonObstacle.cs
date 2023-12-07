using DG.Tweening;
using UnityEngine;

public class PistonObstacle : MonoBehaviour
{
    [SerializeField] private float moveDistance = 4f;
    [SerializeField] private float pullBackDuration = 2f;
    [SerializeField] private float pushDuration = 1f;

    private Tween pistonTween;

    private void Start()
    {
        MovePiston();
    }

    private void MovePiston()
    {
        Vector3 initialPosition = transform.localPosition;

        pistonTween = transform.DOLocalMoveY(initialPosition.y - moveDistance, pullBackDuration)
            .SetEase(Ease.InOutQuad)
            .SetDelay(0.3f)
            .OnComplete(() =>
            {
                pistonTween = transform.DOLocalMoveY(initialPosition.y, pushDuration)
                    .SetEase(Ease.OutBounce)
                    .SetDelay(1f)
                    .OnComplete(() =>
                    {
                        MovePiston();
                    });
            });
    }

    private void OnDestroy()
    {
        if (pistonTween != null)
        {
            pistonTween.Kill();
        }
    }
}
