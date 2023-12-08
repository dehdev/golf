using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBouncePlayer : MonoBehaviour
{
    [SerializeField] private float force = 5f;
    private Vector3 hitDirection;

    private bool isEnabled;

    private void Awake()
    {
        isEnabled = true;
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isEnabled)
        {
            return;
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                hitDirection = contact.normal;
                Vector3 appliedForce = -hitDirection * force;
                appliedForce.y = Mathf.Max(0, appliedForce.y);
                collision.gameObject.GetComponent<PlayerController>().PlayerHitObstacle(appliedForce);
                return;
            }
        }
    }
}
