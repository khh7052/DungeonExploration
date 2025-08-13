using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string itemDescription;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public bool isEquippable = true; // 아이템이 장착 가능한지 여부
    public StatModifierData[] statModifiers;

}
