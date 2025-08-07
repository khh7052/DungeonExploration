using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputManager input;
    private Rigidbody rigd;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayerMask;

    private bool isGrounded = true;
    private bool isJumping = false;

    private void Start()
    {
        input = InputManager.Instance;
        rigd = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        Move();

        if(input.JumpInput)
        {
            Jump();
        }
    }

    public void HandleIdle()
    {
        Vector3 velocity = Vector3.zero;
        velocity.y = rigd.velocity.y; // Y축 속도 유지
        rigd.velocity = velocity;
    }

    public void Move()
    {
        Vector2 moveInput = input.MoveInput;
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 velocity = moveSpeed * moveDirection;
        velocity.y = rigd.velocity.y; // Y축 속도 유지
        rigd.velocity = velocity;
        // transform.forward = moveDirection; // 캐릭터가 이동 방향을 바라보도록 설정
    }

    public void Jump()
    {
        if (!isGrounded) return;
        rigd.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (groundLayerMask.value == (groundLayerMask.value | 1 << collision.gameObject.layer))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (groundLayerMask.value == (groundLayerMask.value | 1 << collision.gameObject.layer))
        {
            isGrounded = false;
        }
    }

}
