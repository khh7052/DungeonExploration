using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private Vector3 jumpDirection = Vector3.up;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
    [SerializeField] private SoundData bounceSFX;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            Vector3 force = jumpDirection.normalized * jumpForce;
            anim.SetTrigger(AnimatorHash.BounceHash);
            AudioManager.Instance.PlaySFX(bounceSFX);

            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerMovementController player = collision.gameObject.GetComponent<PlayerMovementController>();
                player.AddExternalVelocity(force, forceMode);
            }
            else
            {
                collision.rigidbody.AddForce(force, forceMode);
            }
        }
    }
}
