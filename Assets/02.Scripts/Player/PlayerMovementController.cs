using UnityEngine;
using System.Collections;
using Constants;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [SerializeField] private float forwardDirectionLerpSpeed = 2f;
    [SerializeField] private float moveAnimLerpSpeed = 2f;

    private Rigidbody rigd;
    private AnimationHandler animHandler;
    private InputManager input;
    private ThirdPersonCamera thirdPersonCamera;
    private CharacterStats characterStats;

    private Vector3 moveDirection;
    private float moveAnimBlend = 0f;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isDashing = false;

    private float dashCooldown = 1f;
    private float lastDashTime = -1f;

    public float MoveSpeed => characterStats.GetStat(StatType.MoveSpeed).FinalValue;
    public float JumpForce => characterStats.GetStat(StatType.JumpForce).FinalValue;
    public float DashForce => characterStats.GetStat(StatType.DashForce).FinalValue;
    public float DashDuration => characterStats.GetStat(StatType.DashDuration).FinalValue;

    private void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        animHandler = GetComponentInChildren<AnimationHandler>();
        input = InputManager.Instance;
        thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();
        characterStats = GetComponent<PlayerController>().characterStats; // PlayerController에서 공유
    }

    private void Update()
    {
        isGrounded = CheckGround();

        if (isDashing)
        {
            Vector3 v = rigd.velocity;
            v.y = 0;
            rigd.velocity = v;
            return;
        }

        Move();
        if (input.DashInput)
            Dash();

        if (input.JumpInput && isGrounded && !isJumping)
            Jump();

        animHandler.Jump(!isGrounded);
    }

    private void Move()
    {
        Vector2 moveInput = input.MoveInput;
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        moveDirection = thirdPersonCamera.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        Vector3 velocity = MoveSpeed * moveDirection;
        velocity.y = rigd.velocity.y;
        rigd.velocity = velocity;

        UpdateForwardDirection();
        UpdateMoveAnimationBlend();
    }

    private void UpdateForwardDirection()
    {
        if (moveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, forwardDirectionLerpSpeed * Time.deltaTime);
    }

    private void UpdateMoveAnimationBlend()
    {
        float target = moveDirection == Vector3.zero ? 0f : 1f;
        moveAnimBlend = Mathf.Lerp(moveAnimBlend, target, Time.deltaTime * moveAnimLerpSpeed);
        animHandler.MoveSpeed(moveAnimBlend);
    }

    private void Dash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;
        if (moveDirection == Vector3.zero) return;

        isDashing = true;
        lastDashTime = Time.time;

        rigd.velocity = Vector3.zero;
        rigd.AddForce(moveDirection * DashForce, ForceMode.VelocityChange);

        StartCoroutine(EndDashAfterDelay());
    }

    private IEnumerator EndDashAfterDelay()
    {
        animHandler.Dash(true);
        yield return new WaitForSeconds(DashDuration);
        isDashing = false;
        animHandler.Dash(false);
        animHandler.Jump(!isGrounded);
    }

    private void Jump()
    {
        if (!isGrounded) return;
        if (isJumping) return;

        rigd.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
        isJumping = true;
        animHandler.Jump(true);
    }

    private bool CheckGround()
    {
        return Physics.SphereCast(groundCheckPoint.position, groundCheckRadius, Vector3.down, out _, groundCheckDistance, groundLayerMask);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckGround())
        {
            animHandler.Jump(false);
            isJumping = false;
        }
    }
}
