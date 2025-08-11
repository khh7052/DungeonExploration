using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Constants;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    private InputManager input;
    private UIManager uiManager;
    private Rigidbody rigd;
    private AnimationHandler animHandler;

    [Header("Move")]
    [SerializeField] private LayerMask groundLayerMask; 
    [SerializeField] private Transform groundCheckPoint; // �� ��ġ ������
    [SerializeField] private float groundCheckRadius = 0.3f; // ��ü ������
    [SerializeField] private float groundCheckDistance = 0.2f; // SphereCast �Ÿ�

    [SerializeField] private float forwardDirectionLerpSpeed = 2f;
    [SerializeField] private float moveAnimLerpSpeed = 2f;
    private Vector3 moveDirection;
    private float moveAnimBlend = 0;

    private bool isGrounded = true;
    private bool isJumping = false;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 1f;
    private float lastDashTime = -1f;
    private bool isDashing = false;

    [Header("Climb")]
    [SerializeField] private float climbForwardCheckDistance = 0.5f; // Ŭ���̹� üũ �Ÿ�
    [SerializeField] private float climbDownCheckDistance = 1f; // Ŭ���̹� üũ �Ÿ�
    [SerializeField] private Vector3 climbDownCheckOffset = new(0, 1f, 0); // Ŭ���̹� üũ ��ü ������
    [SerializeField] private Transform climbCheckPoint; // Ŭ���̹� üũ ��ġ
    [SerializeField] private LayerMask climbableLayerMask; // Ŭ���̹� ������ ���̾� ����ũ
    private bool isClimbing = false; // Ŭ���̹� ����
    private Vector3 climbPoint;


    [Header("Stats")]
    [SerializeField] private Stat[] initStats;
    public CharacterStats characterStats;

    public int Health
    {
        get => (int)characterStats.GetStat(StatType.CurrentHP).FinalValue;
    }

    public int MaxHealth
    {
        get => (int)characterStats.GetStat(StatType.MaxHP).FinalValue;
    }
    public float MoveSpeed
    {
        get => characterStats.GetStat(StatType.MoveSpeed).FinalValue;
        set => characterStats.GetStat(StatType.MoveSpeed).SetBaseValue(value);
    }
    public float JumpForce
    {
        get => characterStats.GetStat(StatType.JumpForce).FinalValue;
        set => characterStats.GetStat(StatType.JumpForce).SetBaseValue(value);
    }

    public float DashForce
    {
        get => characterStats.GetStat(StatType.DashForce).FinalValue;
        set => characterStats.GetStat(StatType.DashForce).SetBaseValue(value);
    }

    public float DashDuration
    {
        get => characterStats.GetStat(StatType.DashDuration).FinalValue;
        set => characterStats.GetStat(StatType.DashDuration).SetBaseValue(value);
    }

    private PlayerInventoryController inventoryController;
    public PlayerInventoryController InventoryController => inventoryController;

    private PlayerLookController lookController;


    private void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        animHandler = GetComponentInChildren<AnimationHandler>();
        inventoryController = GetComponent<PlayerInventoryController>();
        lookController = GetComponent<PlayerLookController>();

        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ��� ���·� ����

        // ĳ���� ���� �ʱ�ȭ
        characterStats = new();
        foreach (var stat in initStats)
            characterStats.AddStat(new(stat.Type, stat.BaseValue));
    }

    private void Start()
    {
        input = InputManager.Instance;
        uiManager = UIManager.Instance;
    }

    private void Update()
    {
        Debug.Log(isClimbing);

        if (!isClimbing)
        {
            isGrounded = CheckGroundObject(); // �� �����Ӹ��� ���� üũ
            Move();

            if (input.DashInput)
                Dash();

            if (input.JumpInput)
            {
                if (!ClimbIdle())
                    Jump();
            }

            animHandler.Jump(!isGrounded);
        }
        else
        {
            if (input.JumpInput)
                ClimbUp();
        }
    }

    public void Move()
    {
        if (isDashing)
        {
            Vector3 v = rigd.velocity;
            v.y = 0;
            rigd.velocity = v; // ��� �߿��� ��� �������� �ӵ� ����
            return; // ��� ���̸� �̵� ����
        }

        Vector2 moveInput = input.MoveInput;
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        moveDirection = lookController.CameraTransform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        Vector3 velocity = MoveSpeed * moveDirection;
        velocity.y = rigd.velocity.y;
        rigd.velocity = velocity;
        Debug.Log($"Move Direction: {moveDirection}, Velocity: {rigd.velocity}");

        UpdateForwardDirection();
        UpdateMoveAnimationBlend();
    }

    bool ClimbIdle()
    {
        Ray forwardRay = new(climbCheckPoint.position, climbCheckPoint.forward * climbForwardCheckDistance);
        Ray downRay = new(climbCheckPoint.position + transform.TransformDirection(climbDownCheckOffset), Vector3.down * climbDownCheckDistance);

        bool isForwardHit = Physics.Raycast(forwardRay, out RaycastHit forwardHit, climbForwardCheckDistance, climbableLayerMask);
        bool isDownHit = Physics.Raycast(downRay, out RaycastHit downHit, climbDownCheckDistance, climbableLayerMask);

        Debug.Log($"Forward Hit: {isForwardHit}, Down Hit: {isDownHit}");
        if (isForwardHit && isDownHit)
        {
            isJumping = false;
            isClimbing = true; // Ŭ���̹� ������ �������� ������ Ŭ���̹� ���� ����
            animHandler.Jump(false);
            animHandler.Climb(true);
            rigd.isKinematic = true; // Ŭ���̹� �߿��� ���� ������ ������ ���� �ʵ��� ����
            climbPoint = new(forwardHit.point.x, downHit.point.y + 1f, downHit.point.z); // Ŭ���̹� ��ġ ����
            return true;
        }
        else
        {
            isClimbing = false; // Ŭ���̹� ������ �������� ������ Ŭ���̹� ���� ����
            rigd.isKinematic = false; // Ŭ���̹��� ������ ���� ������ ������ �޵��� ����
            animHandler.Climb(false); // Ŭ���̹� �ִϸ��̼� ����
        return false;
        }
    }

    void ClimbUp()
    {
        if (!isClimbing) return;

        // Ŭ���̹� �ִϸ��̼��� Ʈ���ŷ� ����
        animHandler.Climb(false);
    }

    public void ClimbUpEnd()
    {
        isClimbing = false;
        rigd.isKinematic = false; // Ŭ���̹��� ������ ���� ������ ������ �޵��� ����
        transform.position = climbPoint; // Ŭ���̹� ��ġ�� �̵�
    }

    void UpdateMoveAnimationBlend()
    {
        float target = moveDirection == Vector3.zero ? 0f : 1f; // �̵� ������ ������ 0, ������ 1

        if (Mathf.Approximately(moveAnimBlend, target)) return; // ���� ���� ��ǥ ���� ���� ������ ������Ʈ���� ����

        moveAnimBlend = Mathf.Lerp(moveAnimBlend, target, Time.deltaTime * moveAnimLerpSpeed);
        animHandler.MoveSpeed(moveAnimBlend);
    }

    void UpdateForwardDirection()
    {
        if (moveDirection == Vector3.zero) return; // �̵� ������ ������ ������Ʈ���� ����

        // ���� ȸ�� �ӵ��� ���� ȸ��
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, forwardDirectionLerpSpeed * Time.deltaTime);
    }

    public void Dash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;
        if (moveDirection == Vector3.zero) return;

        isDashing = true;
        lastDashTime = Time.time;

        rigd.velocity = Vector3.zero; // ���� �̵� �ӵ� ����
        rigd.AddForce(moveDirection * DashForce, ForceMode.VelocityChange);

        StartCoroutine(EndDashAfterDelay());
    }

    private IEnumerator EndDashAfterDelay()
    {
        animHandler.Dash(true); // ��� �ִϸ��̼� ����
        yield return new WaitForSeconds(DashDuration);
        isDashing = false;
        animHandler.Dash(false); // ��� �ִϸ��̼� ����
        animHandler.Jump(!isGrounded); // ��� �� ���� ���·� �ִϸ��̼� ����
    }

    public void Jump()
    {
        if (!isGrounded) return;
        if (isJumping) return;
        rigd.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
        isJumping = true;
        animHandler.Jump(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckGroundObject())
        {
            animHandler.Jump(false);
            isJumping = false; // ���� �� ���� ���� �ʱ�ȭ
        }
    }

    private bool CheckGroundObject()
        => Physics.SphereCast(groundCheckPoint.position, groundCheckRadius, Vector3.down, out _, groundCheckDistance, groundLayerMask);

    private bool IsSurfaceAngleValid(Collision collision, float maxAngle = 90f)
    {
        foreach (var contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) <= maxAngle)
                return true;
        }
        return false;
    }

    private bool IsGroundCollision(Collision c) =>
    (groundLayerMask & (1 << c.gameObject.layer)) != 0;

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius); // ��ü �׸���

        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheckPoint.position, Vector3.down * groundCheckDistance); // SphereCast ���� �׸���

        if (climbCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(climbCheckPoint.position, climbCheckPoint.forward * climbForwardCheckDistance); // Ŭ���̹� üũ ���� ���� �׸���
            Gizmos.DrawRay(climbCheckPoint.position + transform.TransformDirection(climbDownCheckOffset), Vector3.down * climbDownCheckDistance); // Ŭ���̹� üũ �Ʒ� ���� �׸���

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(climbPoint, 0.5f); // Ŭ���̹� üũ ��ü �׸���
        }
    }


}
