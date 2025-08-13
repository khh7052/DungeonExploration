using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform dropTransform;
    [SerializeField] private SoundData itemUseSFX;

    private InputManager input;
    private PlayerController playerController;

    private void Awake()
    {
        input = InputManager.Instance;
        playerController = GetComponent<PlayerController>();

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
            AudioManager.Instance.PlaySFX(itemUseSFX);
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
        AudioManager.Instance.PlaySFX(itemUseSFX);
    }

    private void Use()
    {
        if (inventory == null) return;
        inventory.Use(playerController.characterStats);
        AudioManager.Instance.PlaySFX(itemUseSFX);
    }
}
