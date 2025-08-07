using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "Data/StatModifierData")]
public class StatModifierData : ScriptableObject
{
    public StatType statType;
    public ModifierType modifierType;
    public float value;
    public object source; // ������, ���� �� ��ó
}
