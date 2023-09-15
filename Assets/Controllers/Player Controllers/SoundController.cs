using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource src;
    [SerializeField] private AudioClip idleSound, ballHit, collisionSound;
    [SerializeField] private float idleSoundVolume = 1;
    [SerializeField] private float ballHitVolume = 1;
    [SerializeField] private float collisionSoundVolume = 0.2f;

    public void PlayIdle()
    {
        // Play the idle sound without interrupting other sounds.
        src.PlayOneShot(idleSound, idleSoundVolume);
    }

    public void PlayBallHit()
    {
        // Play the ball hit sound without interrupting other sounds.
        src.PlayOneShot(ballHit, ballHitVolume);
    }

    public void PlayCollisionSound(float impactMultiplier)
    {
        // Play the collision sound without interrupting other sounds.
        float clampedVolume = Mathf.Clamp(collisionSoundVolume * impactMultiplier, 0.1f, collisionSoundVolume);
        src.PlayOneShot(collisionSound, clampedVolume);
    }
}
