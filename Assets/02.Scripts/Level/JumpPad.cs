using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private Vector3 jumpDirection = Vector3.up;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
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

            if (collision.gameObject.TryGetComponent(out PlayerMovementController player))
            {
                player.AddExternalVelocity(force, forceMode);
            }
            else
            {
                collision.rigidbody.AddForce(force, forceMode);
            }
        }
    }
}
