using UnityEngine;
using Constants;

public class PlayerController : MonoBehaviour, IDamageable
{
    private Rigidbody rigd;
    public Rigidbody RigidBody { get => rigd; }

    [Header("Hit")]
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private SoundData damageSFX;

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
    public float MaxHealth
    {
        get => characterStats.GetStat(StatType.MaxHP).FinalValue;
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

    public void TakeDamage(float damage)
    {
        characterStats.GetStat(StatType.CurrentHP).SetBaseValue(Health - damage);
        ObjectPoolingManager.Instance.Get(damageEffect, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(damageSFX);

        if (Health <= 0)
        {
            GameManager.Instance.Respawn();
        }
    }
}
