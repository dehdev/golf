using UnityEngine;
using DG.Tweening;

public class FlagController : MonoBehaviour
{
    [SerializeField] private float raiseHeight = 2f;
    [SerializeField] private BoxCollider raiseFlagCollider;
    [SerializeField] private BoxCollider lowerFlagCollider;

    [SerializeField] private Transform flagPole;
    [SerializeField] private Transform flag;

    private Vector3 initialFlagPosition;
    private Vector3 initialFlagPolePosition;

    private void Start()
    {
        initialFlagPosition = flag.transform.position;
        initialFlagPolePosition = flagPole.transform.position;
    }

    public void OnRaiseTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaiseFlag();
        }
    }

    public void OnLowerTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LowerFlag();
        }
    }

    private void RaiseFlag()
    {
        flag.DOMoveY(flag.transform.position.y + raiseHeight, 1f).SetEase(Ease.OutQuad);
        flagPole.DOMoveY(flagPole.transform.position.y + raiseHeight, 1f).SetEase(Ease.OutQuad);
    }

    private void LowerFlag()
    {
        flag.DOMoveY(initialFlagPosition.y, 1f).SetEase(Ease.OutQuad);
        flagPole.DOMoveY(initialFlagPolePosition.y, 1f).SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        DOTween.Kill(flag);  // Stop tween for the flag
        DOTween.Kill(flagPole);  // Stop tween for the flag pole
    }
}
