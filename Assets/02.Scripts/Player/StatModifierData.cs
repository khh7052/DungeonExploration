using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

[CreateAssetMenu(menuName = "Data/StatModifierData")]
public class StatModifierData : ScriptableObject
{
    public StatType statType;
    public ModifierType modifierType;
    public float value;
    public bool useDuration = false; // 지속 시간 사용 여부
    public float duration = 0f; // 지속 시간 (초 단위)
    public object source; // 아이템, 버프 등 출처
}
