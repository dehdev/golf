using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    public float moveDistance = 10f;
    public float moveDuration = 5f;

    private Tween cloudTween;

    void Start()
    {
        MoveCloud();
    }

    void MoveCloud()
    {
        cloudTween = transform.DOMoveZ(transform.position.z - moveDistance, moveDuration)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    void OnDestroy()
    {
        // Stop the tween when the object is destroyed
        if (cloudTween != null && cloudTween.IsActive())
        {
            cloudTween.Kill();
        }
    }
}
