using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Constants;

public class PlayerController : MonoBehaviour
{
    private InputManager input;
    private Rigidbody rigd;
    [SerializeField] private Inventory inventory;

    [Header("Move")]
    [SerializeField] private LayerMask groundLayerMask;

    private bool isGrounded = true;
    private bool isJumping = false;

    [Header("Look")]
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 1f;
    private float lastDashTime = -1f;

    [Header("Stats")]
    [SerializeField] private Stat[] initStats;
    public CharacterStats characterStats;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 3f; // 상호작용 거리
    [SerializeField] private LayerMask interactableLayerMask; // 상호작용 가능한 레이어 마스크
    private IInteractable currentInteractable; // 현재 상호작용 가능한 객체

    [Header("UI")]
    [SerializeField] private HUD hud;

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

    public float DashDistance
    {
        get => characterStats.GetStat(StatType.DashDistance).FinalValue;
        set => characterStats.GetStat(StatType.DashDistance).SetBaseValue(value);
    }

    private void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // 커서를 잠금 상태로 설정

        // 캐릭터 스탯 초기화
        characterStats = new();
        foreach (var stat in initStats)
            characterStats.AddStat(new(stat.Type, stat.BaseValue));
    }

    private void Start()
    {
        input = InputManager.Instance;
    }

    private void Update()
    {
        CheckInteractableObject();
        Move();

        if(input.DashInput)
            Dash();

        if (input.JumpInput)
            Jump();

        if (input.InteractInput)
            Interact();
    }

    private void LateUpdate()
    {
        Look();
        UpdatePromptText();
    }

    public void Move()
    {
        Vector2 moveInput = input.MoveInput;
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        moveDirection = thirdPersonCamera.transform.TransformDirection(moveDirection); // 카메라 방향으로 이동
        moveDirection.y = 0; // Y축 방향 제거
        Vector3 velocity = MoveSpeed * moveDirection;
        velocity.y = rigd.velocity.y; // Y축 속도 유지
        rigd.velocity = velocity;

        if(moveDirection == Vector3.zero) return;
        transform.forward = moveDirection; // 캐릭터가 이동 방향을 바라보도록 설정
    }

    public void Dash()
    {
        if (Time.time < lastDashTime + dashCooldown) return; // 대시 쿨타임 확인

        Vector3 dashDirection = thirdPersonCamera.transform.forward; // 카메라 방향으로 대시
        dashDirection.y = 0; // Y축 방향 제거
        dashDirection.Normalize();


        transform.position = transform.position + dashDirection * DashDistance; // 대시 거리만큼 이동
        lastDashTime = Time.time; // 대시 시간 갱신
    }

    public void Jump()
    {
        if (!isGrounded) return;
        rigd.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
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
        if (currentInteractable != null)
        {
            currentInteractable.Interact(this);
            currentInteractable = null; // 상호작용 후 초기화
        }
    }

    void UpdatePromptText()
    {
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
