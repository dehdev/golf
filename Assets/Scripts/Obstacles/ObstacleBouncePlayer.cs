using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBouncePlayer : MonoBehaviour
{
    [SerializeField] private float force = 5f;
    private Vector3 hitDirection;

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.tag == "Player")
            {
                hitDirection = contact.normal;
                collision.gameObject.GetComponent<PlayerController>().PlayerHitObstacle(-hitDirection * force);
                return;
            }
        }
    }
}
