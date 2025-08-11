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
    [SerializeField] private Transform groundCheckPoint; // 발 위치 기준점
    [SerializeField] private float groundCheckRadius = 0.3f; // 구체 반지름
    [SerializeField] private float groundCheckDistance = 0.2f; // SphereCast 거리

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
    [SerializeField] private float climbForwardCheckDistance = 0.5f; // 클라이밍 체크 거리
    [SerializeField] private float climbDownCheckDistance = 1f; // 클라이밍 체크 거리
    [SerializeField] private Vector3 climbDownCheckOffset = new(0, 1f, 0); // 클라이밍 체크 구체 반지름
    [SerializeField] private Transform climbCheckPoint; // 클라이밍 체크 위치
    [SerializeField] private LayerMask climbableLayerMask; // 클라이밍 가능한 레이어 마스크
    private bool isClimbing = false; // 클라이밍 상태
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

        Cursor.lockState = CursorLockMode.Locked; // 커서를 잠금 상태로 설정

        // 캐릭터 스탯 초기화
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
            isGrounded = CheckGroundObject(); // 매 프레임마다 지면 체크
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
            rigd.velocity = v; // 대시 중에는 대시 방향으로 속도 설정
            return; // 대시 중이면 이동 차단
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
            isClimbing = true; // 클라이밍 조건이 충족되지 않으면 클라이밍 상태 종료
            animHandler.Jump(false);
            animHandler.Climb(true);
            rigd.isKinematic = true; // 클라이밍 중에는 물리 엔진의 영향을 받지 않도록 설정
            climbPoint = new(forwardHit.point.x, downHit.point.y + 1f, downHit.point.z); // 클라이밍 위치 설정
            return true;
        }
        else
        {
            isClimbing = false; // 클라이밍 조건이 충족되지 않으면 클라이밍 상태 종료
            rigd.isKinematic = false; // 클라이밍이 끝나면 물리 엔진의 영향을 받도록 설정
            animHandler.Climb(false); // 클라이밍 애니메이션 종료
        return false;
        }
    }

    void ClimbUp()
    {
        if (!isClimbing) return;

        // 클라이밍 애니메이션을 트리거로 설정
        animHandler.Climb(false);
    }

    public void ClimbUpEnd()
    {
        isClimbing = false;
        rigd.isKinematic = false; // 클라이밍이 끝나면 물리 엔진의 영향을 받도록 설정
        transform.position = climbPoint; // 클라이밍 위치로 이동
    }

    void UpdateMoveAnimationBlend()
    {
        float target = moveDirection == Vector3.zero ? 0f : 1f; // 이동 방향이 없으면 0, 있으면 1

        if (Mathf.Approximately(moveAnimBlend, target)) return; // 현재 값과 목표 값이 거의 같으면 업데이트하지 않음

        moveAnimBlend = Mathf.Lerp(moveAnimBlend, target, Time.deltaTime * moveAnimLerpSpeed);
        animHandler.MoveSpeed(moveAnimBlend);
    }

    void UpdateForwardDirection()
    {
        if (moveDirection == Vector3.zero) return; // 이동 방향이 없으면 업데이트하지 않음

        // 현재 회전 속도에 따라 회전
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, forwardDirectionLerpSpeed * Time.deltaTime);
    }

    public void Dash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;
        if (moveDirection == Vector3.zero) return;

        isDashing = true;
        lastDashTime = Time.time;

        rigd.velocity = Vector3.zero; // 기존 이동 속도 제거
        rigd.AddForce(moveDirection * DashForce, ForceMode.VelocityChange);

        StartCoroutine(EndDashAfterDelay());
    }

    private IEnumerator EndDashAfterDelay()
    {
        animHandler.Dash(true); // 대시 애니메이션 시작
        yield return new WaitForSeconds(DashDuration);
        isDashing = false;
        animHandler.Dash(false); // 대시 애니메이션 종료
        animHandler.Jump(!isGrounded); // 대시 후 착지 상태로 애니메이션 설정
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
            isJumping = false; // 착지 시 점프 상태 초기화
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
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius); // 구체 그리기

        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheckPoint.position, Vector3.down * groundCheckDistance); // SphereCast 방향 그리기

        if (climbCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(climbCheckPoint.position, climbCheckPoint.forward * climbForwardCheckDistance); // 클라이밍 체크 전방 방향 그리기
            Gizmos.DrawRay(climbCheckPoint.position + transform.TransformDirection(climbDownCheckOffset), Vector3.down * climbDownCheckDistance); // 클라이밍 체크 아래 방향 그리기

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(climbPoint, 0.5f); // 클라이밍 체크 구체 그리기
        }
    }


}
