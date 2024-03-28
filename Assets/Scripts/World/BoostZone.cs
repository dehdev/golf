using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZone : MonoBehaviour
{
    // The speed boost amount
    public float boostAmount = 10f;

    // Called when another Collider enters this Collider's trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Get the Rigidbody component of the player
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                // Apply the boost to the player's velocity in the forward direction
                playerRigidbody.AddForce(playerRigidbody.transform.position.normalized * boostAmount);
            }
        }
    }
}
