using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip idleSound, ballHit, collisionSound;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnIdleEvent += PlayerController_OnIdleEvent;
        PlayerController.OnBallHit += PlayerController_OnBallHit;
        PlayerController.OnCollisionHit += PlayerController_OnCollisionHit;
    }

    private void PlayerController_OnBallHit(object sender, EventArgs e)
    {
        PlayerController playerController = sender as PlayerController;
        PlaySound(ballHit, playerController.transform.position);
    }

    private void PlayerController_OnCollisionHit(object sender, float e)
    {
        PlayerController playerController = sender as PlayerController;
        PlaySound(collisionSound, playerController.transform.position, e);
    }

    private void PlayerController_OnIdleEvent(object sender, EventArgs e)
    {
        PlayerController playerController = sender as PlayerController;
        PlaySound(idleSound, playerController.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
}
