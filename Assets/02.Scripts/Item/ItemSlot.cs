using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Image itemIcon;

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


}
