using UnityEngine;
using System.Collections;
using Constants;

public class PlayerMovementController : MonoBehaviour
{
    // �÷��̾� �̵�
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    private Vector3 groundNormal;

    [SerializeField] private GameObject footstepEffect;
    [SerializeField] private float forwardDirectionLerpSpeed = 2f;
    [SerializeField] private float moveAnimLerpSpeed = 2f;

    private PlayerController playerController;
    private Rigidbody rigd;
    private AnimationHandler animHandler;
    private InputManager input;
    private ThirdPersonCamera thirdPersonCamera;

    private Vector3 moveDirection;
    private float moveAnimBlend = 0f;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isDashing = false;


    // �뽬
    [SerializeField] private GameObject dashEffect;
    private float dashCooldown = 1f;
    private float lastDashTime = -1f;

    // �� �浹
    private bool isTouchingWall = false;
    private Vector3 wallNormal = Vector3.zero;
    [SerializeField] private float wallMinAngle = 45f; // ������ �����ϴ� �ּ� ����
    [SerializeField] private float wallSlideSpeed = 5f; // �ʴ� �ϰ� �ӵ�

    // �ܺ� �ӵ� ����
    private Vector3 externalVelocity = Vector3.zero;
    [SerializeField] private float externalVelocityDecay = 5f; // �ʴ� ���� �ӵ�

    CharacterStats CharacterStats => playerController.characterStats;

    public float MoveSpeed => CharacterStats.GetStat(StatType.MoveSpeed).FinalValue;
    public float JumpForce => CharacterStats.GetStat(StatType.JumpForce).FinalValue;
    public float DashForce => CharacterStats.GetStat(StatType.DashForce).FinalValue;
    public float DashDuration => CharacterStats.GetStat(StatType.DashDuration).FinalValue;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rigd = GetComponent<Rigidbody>();
        animHandler = GetComponentInChildren<AnimationHandler>();
        input = InputManager.Instance;
        thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();
    }

    private void Update()
    {
        if(playerController.ClimbController.IsClimbing) return;
        isGrounded = IsGrounded();

        ProcessInput();

        if (isDashing)
            SetVelocityYToZero();

        DecayExternalVelocity();
        UpdateAnimationStates();

        Debug.Log($"Velocity {rigd.velocity}, External Velocity: {externalVelocity}");
    }

    private void ProcessInput()
    {
        UpdateMoveDirection();

        if (CanDash())
            Dash();

        if (CanJump())
            Jump();

        if (CanMove())
            ApplyMovement();
    }

    private void UpdateMoveDirection()
    {
        Vector2 moveInput = input.MoveInput;
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        moveDirection = thirdPersonCamera.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;
    }
    private bool CanMove() => !isDashing;

    private bool CanJump() => input.JumpInput && isGrounded && !isJumping;

    private bool CanDash() => input.DashInput && !isDashing && moveDirection != Vector3.zero && Time.time >= lastDashTime + dashCooldown;

    private void DecayExternalVelocity()
    {
        if (externalVelocity.magnitude > 0.01f)
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, Time.deltaTime * externalVelocityDecay);
        else
            externalVelocity = Vector3.zero;
    }

    private void ApplyMovement()
    {
        Vector3 desiredVelocity = moveDirection * MoveSpeed;

        if (isGrounded)
        {
            // Vector3 dir = Vector3.ProjectOnPlane(desiredVelocity, groundNormal); // ���� ������ ������ �������� �̵�
            // desiredVelocity = dir.normalized * MoveSpeed;
            desiredVelocity.y = rigd.velocity.y; // ���� Y�ӵ� ����
        }
        else
        {
            if (isTouchingWall)
            {
                Vector3 slideDirection = Vector3.Cross(wallNormal, Vector3.Cross(Vector3.down, wallNormal)).normalized;
                desiredVelocity = slideDirection * wallSlideSpeed;
            }
            else
            {
                desiredVelocity.y = rigd.velocity.y; // ���� Y�ӵ� ����
            }
        }

        // ���� �ӵ� = �̵� �ӵ� + �ܺ� �ӵ�
        rigd.velocity = desiredVelocity + externalVelocity;

        UpdateForwardRotation();
    }

    // �ܺο��� ȣ���� �߻� �ӵ� ����
    public void AddExternalVelocity(Vector3 force, ForceMode mode = ForceMode.VelocityChange)
    {
        if (mode == ForceMode.VelocityChange)
            externalVelocity += force;
        else
            externalVelocity += force / rigd.mass;

        externalVelocity.y = 0;

        // y �ӵ��� ��ø���� �ʵ��� �ѹ��� ���ϵ��� ����
        Vector3 v = rigd.velocity;
        v.y += force.y;
        rigd.velocity = v;
    }

    private void UpdateForwardRotation()
    {
        if (moveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, forwardDirectionLerpSpeed * Time.deltaTime);
    }

    public void SpawnFootstepEffect()
    {
        if (!isGrounded || footstepEffect == null) return;

        Instantiate(footstepEffect, groundCheckPoint.position, Quaternion.identity);
    }

    private void Jump()
    {
        // AddExternalVelocity(Vector3.up * JumpForce, ForceMode.VelocityChange);
        rigd.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
        isJumping = true;
        animHandler.Jump(true);
    }

    private void Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        rigd.velocity = Vector3.zero;
        rigd.AddForce(moveDirection * DashForce, ForceMode.VelocityChange);

        if (dashEffect != null)
        {
            GameObject effect = Instantiate(dashEffect, transform.position, Quaternion.LookRotation(moveDirection));
        }

        StartCoroutine(EndDashAfterDelay());
        animHandler.Dash(true);
    }

    private IEnumerator EndDashAfterDelay()
    {
        yield return new WaitForSeconds(DashDuration);
        isDashing = false;
        animHandler.Dash(false);
        animHandler.Jump(!isGrounded);
    }

    private bool IsGrounded()
    {
        if (Physics.SphereCast(groundCheckPoint.position, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayerMask))
        {
            // �� ���� ���͸� �߰� (��簡 �ʹ� ���ĸ��� �� �ƴ� ó��)
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle < wallMinAngle) return true;
        }
        return false;
    }

    private bool IsWall(Vector3 normal)
    {
        float angle = Vector3.Angle(Vector3.up, normal);
        return angle >= wallMinAngle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerController.ClimbController.IsClimbing) return;

        if (IsGrounded())
        {
            animHandler.Jump(false);
            isJumping = false;
            groundNormal = collision.GetContact(0).normal;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerController.ClimbController.IsClimbing) return;

        // �� ���� �ӽ� �÷���
        bool touchingWallThisFrame = false;
        Vector3 detectedWallNormal = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (IsWall(contact.normal))
            {
                // �̵� ����� �� �븻�� �ʹ� �ָ� �� ���� �ƴ� ó��(�÷��̾ ������ �־����� ��)
                float angleBetweenMoveAndWall = Vector3.Angle(moveDirection, -contact.normal);
                if (angleBetweenMoveAndWall < 90f)
                {
                    touchingWallThisFrame = true;
                    detectedWallNormal = contact.normal;
                    break;
                }
            }
        }

        isTouchingWall = touchingWallThisFrame;
        if (touchingWallThisFrame)
            wallNormal = detectedWallNormal;
        else
            wallNormal = Vector3.zero;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (playerController.ClimbController.IsClimbing) return;
        isTouchingWall = false;
        wallNormal = Vector3.zero;
    }

    private void UpdateAnimationStates()
    {
        float target = moveDirection == Vector3.zero ? 0f : 1f;
        moveAnimBlend = Mathf.Lerp(moveAnimBlend, target, Time.deltaTime * moveAnimLerpSpeed);
        animHandler.MoveSpeed(moveAnimBlend);

        if (playerController.ClimbController.IsClimbing)
        {
            animHandler.Jump(false);
        }
        else
        {
            animHandler.Jump(!isGrounded);
        }
        animHandler.Dash(isDashing);
    }

    private void SetVelocityYToZero()
    {
        Vector3 v = rigd.velocity;
        v.y = 0;
        rigd.velocity = v;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheckPoint.position, Vector3.down * groundCheckDistance);

        if (rigd != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, rigd.velocity.normalized * 2f);
        }

        if (isTouchingWall)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, wallNormal * 2f);

            Vector3 slideDirection = Vector3.Cross(wallNormal, Vector3.Cross(Vector3.down, wallNormal)).normalized;
            Gizmos.DrawRay(transform.position, slideDirection * 5f);
        }
    }
}
