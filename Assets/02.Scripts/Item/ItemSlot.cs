using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Outline outline;

    public ItemData ItemData
    {
        get => itemData;
        set
        {
            itemData = value;
            UpdateIconImage();
        }
    }

    void UpdateIconImage()
    {
        itemIcon.sprite = itemData != null ? itemData.itemIcon : null;
    }

    public void Select(bool select)
    {
        if (outline != null)
            outline.enabled = select;
    }

    public void Drop(Vector3 dropPosition)
    {
        if (itemData != null)
        {
            Instantiate(itemData.itemPrefab, dropPosition, Quaternion.identity);
            ItemData = null;
        }
    }
}
