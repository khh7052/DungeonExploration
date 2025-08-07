using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputManager input;
    private Rigidbody rigd;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayerMask;

    private bool isGrounded = true;
    private bool isJumping = false;

    [Header("Look")]
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;




    private void Start()
    {
        input = InputManager.Instance;
        rigd = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ��� ���·� ����
    }

    private void Update()
    {
        Move();

        if(input.JumpInput)
        {
            Jump();
        }
    }

    private void LateUpdate()
    {
        Look();
    }

    public void HandleIdle()
    {
        Vector3 velocity = Vector3.zero;
        velocity.y = rigd.velocity.y; // Y�� �ӵ� ����
        rigd.velocity = velocity;
    }

    public void Move()
    {
        Vector2 moveInput = input.MoveInput;
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        moveDirection = thirdPersonCamera.transform.TransformDirection(moveDirection); // ī�޶� �������� �̵�
        moveDirection.y = 0; // Y�� ���� ����
        Vector3 velocity = moveSpeed * moveDirection;
        velocity.y = rigd.velocity.y; // Y�� �ӵ� ����
        rigd.velocity = velocity;
        // transform.forward = moveDirection; // ĳ���Ͱ� �̵� ������ �ٶ󺸵��� ����
    }

    public void Jump()
    {
        if (!isGrounded) return;
        rigd.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    public void Look()
    {
        float mouseX = input.LookInput.x;
        float mouseY = input.LookInput.y;

        thirdPersonCamera.Rotate(mouseX, mouseY);
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
