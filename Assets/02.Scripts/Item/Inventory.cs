using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private UIManager uiManager;

    [SerializeField] private int inventorySize = 20; // 기본 인벤토리 크기
    [SerializeField] private Transform itemSlotParent; // 아이템 슬롯이 배치될 부모
    [SerializeField] private ItemSlot itemSlotPrefab; // 아이템 슬롯 프리팹
    private ItemSlot[] itemSlots;
    private int selectedSlotIndex; // 현재 선택된 슬롯 인덱스

    public ItemSlot SelectedItemSlot
    {
        get
        {
            if (selectedSlotIndex < 0 || selectedSlotIndex >= itemSlots.Length)
                return null; // 유효하지 않은 인덱스일 경우 null 반환
            return itemSlots[selectedSlotIndex]; // 선택된 슬롯 반환
        }
    }

    public ItemSlot[] ItemSlots { get => itemSlots; }

    private void Awake()
    {
        itemSlots = new ItemSlot[inventorySize];
        selectedSlotIndex = -1;

        for (int i = 0; i < inventorySize; i++)
        {
            ItemSlot slot = Instantiate(itemSlotPrefab, itemSlotParent);
            slot.gameObject.name = "ItemSlot_" + i;
            itemSlots[i] = slot;
        }
    }

    private void Start()
    {
        uiManager = UIManager.Instance;
    }

    public bool AddItem(ItemData itemData)
    {
        if (SelectedItemSlot != null && SelectedItemSlot.ItemData == null)
        {
            SelectedItemSlot.ItemData = itemData; // 선택된 슬롯에 아이템 추가
            return true; // 아이템 추가 성공
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].ItemData == null)
            {
                itemSlots[i].ItemData = itemData;
                return true; // 아이템 추가 성공
            }
        }
        return false; // 인벤토리가 가득 찼음
    }

    public bool RemoveItem(ItemData itemData)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].ItemData == itemData)
            {
                itemSlots[i].ItemData = null;
                return true; // 아이템 제거 성공
            }
        }
        return false; // 해당 아이템이 인벤토리에 없음
    }

    public void Drop(Vector3 dropPosition)
    {
        SelectedItemSlot?.Drop(dropPosition); // 선택된 슬롯에서 아이템 드롭
        uiManager.HUD.UpdateDescriptionText("");
    }

    public ItemData GetItem(int index)
    {
        if (index < 0 || index >= itemSlots.Length)
        {
            return null; // 유효하지 않은 인덱스
        }
        return itemSlots[index].ItemData; // 해당 인덱스의 아이템 데이터 반환
    }

    public void ClearInventory()
    {
        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].ItemData = null; // 모든 슬롯의 아이템 데이터 초기화
    }

    public void SelectItemSlot(int newIndex)
    {
        if (newIndex < 0 || newIndex >= itemSlots.Length) return;

        selectedSlotIndex = selectedSlotIndex != newIndex ? newIndex : -1;

        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].Select(i == selectedSlotIndex); // 선택된 슬롯에만 선택 표시

        Debug.Log($"Selected slot index: {selectedSlotIndex}");
        string description = selectedSlotIndex < 0? "" : SelectedItemSlot.ItemData?.itemDescription;
        uiManager.HUD.UpdateDescriptionText(description);
    }

}
