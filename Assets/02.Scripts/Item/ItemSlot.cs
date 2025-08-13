using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private Color selectedColor = Color.yellow; // ���õ� ������ ����
    [SerializeField] private Color defaultColor = Color.white; // �⺻ ����
    [SerializeField] private Outline outline;
    private bool isEquipped = false;

    public ItemData ItemData
    {
        get => itemData;
        set
        {
            itemData = value;
            UpdateIconImage();
        }
    }

    public bool IsEquipped
    {
        get => isEquipped;
    }

    void UpdateIconImage()
    {
        itemIcon.sprite = itemData != null ? itemData.itemIcon : null;
    }

    public void ToggleEquipped()
    {
        isEquipped = !isEquipped;
        itemBackground.color = isEquipped ? selectedColor : defaultColor; // ���� ����
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
            GameObject go = ObjectPoolingManager.Instance.Get(itemData.itemPrefab, dropPosition, Quaternion.identity);
            ItemObject itemObject = go.GetComponent<ItemObject>();
            itemObject.ItemData = itemData;
            ItemData = null;
        }
    }
}
