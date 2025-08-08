using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;

    public ItemData ItemData
    {
        get => itemData;
        set
        {
            itemData = value;
            if (itemData != null)
            {
                itemData.itemPrefab = gameObject;
            }
        }
    }

    public void Interact(PlayerController controller)
    {
        controller.AddItem(itemData);
        gameObject.SetActive(false);
    }
}
