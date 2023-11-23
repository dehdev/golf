using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    public float moveDistance = 10f;
    public float moveDuration = 5f;

    void Start()
    {
        MoveCloud();
    }

    void MoveCloud()
    {
        transform.DOMoveZ(transform.position.z - moveDistance, moveDuration)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}
