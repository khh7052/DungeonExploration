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
    [SerializeField] private float interactDistance = 3f; // ��ȣ�ۿ� �Ÿ�
    [SerializeField] private LayerMask interactableLayerMask; // ��ȣ�ۿ� ������ ���̾� ����ũ
    private IInteractable currentInteractable; // ���� ��ȣ�ۿ� ������ ��ü

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

        input.InteractAction += Interact; // �κ��丮 ��ȣ�ۿ� �׼� ���
        input.SelectAction += Select; // �κ��丮 ���� �׼� ���
        input.DropAction += Drop; // �κ��丮 ��� �׼� ���
        input.UseAction += Use; // �κ��丮 ��� �׼� ���
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
            rigd.velocity = v; // ��� �߿��� ��� �������� �ӵ� ����
            return; // ��� ���̸� �̵� ����
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
        currentInteractable = null; // ��ȣ�ۿ� �� �ʱ�ȭ
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
