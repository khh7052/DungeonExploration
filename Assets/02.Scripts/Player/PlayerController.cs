using UnityEngine;
using Constants;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigd;
    public Rigidbody RigidBody { get => rigd; }

    [Header("Stats")]
    [SerializeField] private Stat[] initStats;
    public CharacterStats characterStats;

    private PlayerInventoryController inventoryController;
    public PlayerInventoryController InventoryController { get => inventoryController; }

    private PlayerClimbController climbController;
    public PlayerClimbController ClimbController { get => climbController; }

    private PlayerMovementController movementController;
    public PlayerMovementController MovementController { get => movementController; }

    public float Health
    {
        get => characterStats.GetStat(StatType.CurrentHP).FinalValue;
    }

    private void Awake()
    {
        rigd = GetComponent<Rigidbody>();


        inventoryController = GetComponent<PlayerInventoryController>();
        climbController = GetComponent<PlayerClimbController>();
        movementController = GetComponent<PlayerMovementController>();

        Cursor.lockState = CursorLockMode.Locked;

        InitializeStats();
    }

    private void InitializeStats()
    {
        characterStats = new();
        foreach (var stat in initStats)
            characterStats.AddStat(new(stat.Type, stat.BaseValue));
    }
}
