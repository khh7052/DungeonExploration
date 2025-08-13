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
    public bool isEquippable = true; // �������� ���� �������� ����
    public StatModifierData[] statModifiers;

}
