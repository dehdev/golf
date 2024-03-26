using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObstacle : MonoBehaviour
{
    public static EventHandler OnTargetHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnTargetHit?.Invoke(this, EventArgs.Empty);
        }
    }
}

