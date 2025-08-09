using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Constants;

public class PlayerController : MonoBehaviour
{
    private InputManager input;
    private UIManager uiManager;
    private Rigidbody rigd;
    private AnimationHandler animHandler;

    [Header("Move")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float forwardDirectionLerpSpeed = 2f;
    [SerializeField] private float moveAnimLerpSpeed = 2f;
    private Vector3 moveDirection;
    private float moveAnimBlend = 0;

    private bool isGrounded = true;

    [Header("Look")]
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 1f;
    private float lastDashTime = -1f;
    private bool isDashing = false;

    [Header("Stats")]
    [SerializeField] private Stat[] initStats;
    public CharacterStats characterStats;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 3f; // 상호작용 거리
    [SerializeField] private LayerMask interactableLayerMask; // 상호작용 가능한 레이어 마스크
    private IInteractable currentInteractable; // 현재 상호작용 가능한 객체

    [Header("Inventory")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform dropTransform;

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

    private void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        animHandler = GetComponent<AnimationHandler>();

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

        input.InteractAction += Interact; // 인벤토리 상호작용 액션 등록
        input.SelectAction += Select; // 인벤토리 선택 액션 등록
        input.DropAction += Drop; // 인벤토리 드롭 액션 등록
        input.UseAction += Use; // 인벤토리 사용 액션 등록
    }

    private void Update()
    {
        CheckInteractableObject();
        Move();

        if(input.DashInput)
            Dash();

        if (input.JumpInput)
            Jump();
    }

    private void LateUpdate()
    {
        Look();
        UpdatePromptText();
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
        moveDirection = thirdPersonCamera.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        Vector3 velocity = MoveSpeed * moveDirection;
        velocity.y = rigd.velocity.y;
        rigd.velocity = velocity;

        UpdateForwardDirection();
        UpdateMoveAnimationBlend();
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
        rigd.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
        isGrounded = false;
    }

    public void Look()
    {
        float mouseX = input.LookInput.x;
        float mouseY = input.LookInput.y;

        thirdPersonCamera.Rotate(mouseX, mouseY);
    }

    public void Interact()
    {
        if (currentInteractable == null) return;
        currentInteractable.Interact(this);
        currentInteractable = null; // 상호작용 후 초기화
    }

    public void Select(int inputNumber)
    {
        if (inventory == null) return;
        inventory.SelectItemSlot(inputNumber - 1);
    }

    public void Drop() => inventory.Drop(dropTransform.position);

    void UpdatePromptText()
    {
        HUD hud = uiManager.HUD;

        if (currentInteractable != null)
            hud.UpdatePromptText(currentInteractable.GetPrompt());
        else
            hud.UpdatePromptText("");
    }

    void CheckInteractableObject()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 카메라 중앙 위치
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
                currentInteractable = interactable; // 현재 상호작용 가능한 객체 설정
            else
                currentInteractable = null; // 상호작용 가능한 객체가 없으면 초기화
        }
        else
            currentInteractable = null; // 레이캐스트에 맞는 객체가 없으면 초기화
    }
    

    public void AddItem(ItemData itemData)
    {
        if (inventory == null || itemData == null) return;
        
        if (inventory.AddItem(itemData))
        {
            Debug.Log($"Added item: {itemData.itemName}");
        }
        else
        {
            Debug.LogWarning("Inventory is full or item could not be added.");
        }
    }

    public void Use()
    {
        if (inventory == null) return;
        inventory.Use(characterStats);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (groundLayerMask.value == (groundLayerMask.value | 1 << collision.gameObject.layer))
        {
            isGrounded = true;
            animHandler.Jump(false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (groundLayerMask.value == (groundLayerMask.value | 1 << collision.gameObject.layer))
        {
            isGrounded = false;
            animHandler.Jump(true);
        }
    }
}
