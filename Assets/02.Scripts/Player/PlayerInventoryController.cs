using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform dropTransform;

    private InputManager input;
    private CharacterStats characterStats;

    private void Awake()
    {
        input = InputManager.Instance;
        characterStats = GetComponent<PlayerController>().characterStats;

        input.SelectAction += Select;
        input.DropAction += Drop;
        input.UseAction += Use;
    }

    public bool AddItem(ItemData itemData)
    {
        if (inventory == null || itemData == null) return false;
        if (inventory.AddItem(itemData))
        {
            Debug.Log($"Added item: {itemData.itemName}");
            return true;
        }
        return false;
    }

    private void Select(int inputNumber)
    {
        if (inventory == null) return;
        inventory.SelectItemSlot(inputNumber - 1);
    }

    private void Drop()
    {
        if (inventory == null) return;
        inventory.Drop(dropTransform.position);
    }

    private void Use()
    {
        if (inventory == null) return;
        inventory.Use(characterStats);
    }
}
