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
    [SerializeField] private float interactDistance = 3f; // ��ȣ�ۿ� �Ÿ�
    [SerializeField] private LayerMask interactableLayerMask; // ��ȣ�ۿ� ������ ���̾� ����ũ
    private IInteractable currentInteractable; // ���� ��ȣ�ۿ� ������ ��ü

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
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ��� ���·� ����

        // ĳ���� ���� �ʱ�ȭ
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
        moveDirection = thirdPersonCamera.transform.TransformDirection(moveDirection); // ī�޶� �������� �̵�
        moveDirection.y = 0; // Y�� ���� ����
        Vector3 velocity = MoveSpeed * moveDirection;
        velocity.y = rigd.velocity.y; // Y�� �ӵ� ����
        rigd.velocity = velocity;

        if(moveDirection == Vector3.zero) return;
        transform.forward = moveDirection; // ĳ���Ͱ� �̵� ������ �ٶ󺸵��� ����
    }

    public void Dash()
    {
        if (Time.time < lastDashTime + dashCooldown) return; // ��� ��Ÿ�� Ȯ��

        Vector3 dashDirection = thirdPersonCamera.transform.forward; // ī�޶� �������� ���
        dashDirection.y = 0; // Y�� ���� ����
        dashDirection.Normalize();


        transform.position = transform.position + dashDirection * DashDistance; // ��� �Ÿ���ŭ �̵�
        lastDashTime = Time.time; // ��� �ð� ����
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
            currentInteractable = null; // ��ȣ�ۿ� �� �ʱ�ȭ
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
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // ī�޶� �߾� ��ġ
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
                currentInteractable = interactable; // ���� ��ȣ�ۿ� ������ ��ü ����
            else
                currentInteractable = null; // ��ȣ�ۿ� ������ ��ü�� ������ �ʱ�ȭ
        }
        else
            currentInteractable = null; // ����ĳ��Ʈ�� �´� ��ü�� ������ �ʱ�ȭ
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
