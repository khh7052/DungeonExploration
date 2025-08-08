using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int inventorySize = 20; // �⺻ �κ��丮 ũ��
    [SerializeField] private Transform itemSlotParent; // ������ ������ ��ġ�� �θ�
    [SerializeField] private ItemSlot itemSlotPrefab; // ������ ���� ������
    private ItemSlot[] itemSlots;
    private int selectedSlotIndex; // ���� ���õ� ���� �ε���

    public ItemSlot[] ItemSlots
    {
        get => itemSlots;
    }

    private void Awake()
    {
        itemSlots = new ItemSlot[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            ItemSlot slot = Instantiate(itemSlotPrefab, itemSlotParent);
            slot.gameObject.name = "ItemSlot_" + i;
            itemSlots[i] = slot;
        }
    }

    public bool AddItem(ItemData itemData)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].ItemData == null)
            {
                itemSlots[i].ItemData = itemData;
                return true; // ������ �߰� ����
            }
        }
        return false; // �κ��丮�� ���� á��
    }

    public bool RemoveItem(ItemData itemData)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].ItemData == itemData)
            {
                itemSlots[i].ItemData = null;
                return true; // ������ ���� ����
            }
        }
        return false; // �ش� �������� �κ��丮�� ����
    }

    public ItemData GetItem(int index)
    {
        if (index < 0 || index >= itemSlots.Length)
        {
            return null; // ��ȿ���� ���� �ε���
        }
        return itemSlots[index].ItemData; // �ش� �ε����� ������ ������ ��ȯ
    }

    public void ClearInventory()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].ItemData = null; // ��� ������ ������ ������ �ʱ�ȭ
        }
    }

    public void SelectItemSlot(int newIndex)
    {
        if (newIndex < 0 || newIndex >= itemSlots.Length) return;

        selectedSlotIndex = newIndex;

        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].Select(i == selectedSlotIndex); // ���õ� ���Կ��� ���� ǥ��
    }

}
