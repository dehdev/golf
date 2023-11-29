using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float moveDuration = 5f;

    private Tween cloudTween;

    void Start()
    {
        MoveCloud();
    }

    void MoveCloud()
    {
        cloudTween = transform.DOMoveZ(transform.position.z - moveDistance, moveDuration)
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
