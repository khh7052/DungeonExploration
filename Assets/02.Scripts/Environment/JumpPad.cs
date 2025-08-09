using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private Vector3 jumpDirection = Vector3.up;
    [SerializeField] private float jumpForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            Vector3 force = jumpDirection.normalized * jumpForce;
            collision.rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
