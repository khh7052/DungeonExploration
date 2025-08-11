using UnityEngine;
using Constants;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private Stat[] initStats;
    public CharacterStats characterStats;

    private PlayerInventoryController inventoryController;
    public PlayerInventoryController InventoryController { get => inventoryController; }

    private PlayerClimbController climbController;
    public PlayerClimbController ClimbController { get => climbController; }

    public float Health
    {
        get => characterStats.GetStat(StatType.CurrentHP).FinalValue;
    }

    private void Awake()
    {
        inventoryController = GetComponent<PlayerInventoryController>();
        climbController = GetComponent<PlayerClimbController>();
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
