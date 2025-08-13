using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    private Rigidbody rigd;

    private void Awake()
    {
        rigd = GetComponent<Rigidbody>();
    }

    public ItemData ItemData
    {
        get => itemData;
        set => itemData = value;
    }

    public void Interact(PlayerController controller)
    {
        if (controller.InventoryController.AddItem(itemData))
            gameObject.SetActive(false);
    }

    public string GetPrompt() => $"ащ╠Б E";
}
