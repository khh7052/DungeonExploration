using System.Collections.Generic;
using UnityEngine;
using Constants;
using System;
using System.Collections;

[System.Serializable]
public class Stat
{
    public event Action<float> FinalValueChanged;

    [SerializeField] private StatType type;
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifierData> modifiers = new();

    private float finalValue;
    private bool isDirty = true; // 최종 값이 변경되었는지 여부

    public StatType Type => type;
    public float BaseValue => baseValue;
    public float FinalValue
    {
        get
        {
            if (isDirty)
            {
                RecalculateFinalValue();
                isDirty = false;
            }
            return finalValue;
        }
    }

    public Stat(StatType type, float baseValue = 0f)
    {
        this.type = type;
        this.baseValue = baseValue;
        finalValue = baseValue; // 초기 최종 값은 기본 값과 동일
    }


    public void SetBaseValue(float value)
    {
        if (Mathf.Approximately(baseValue, value)) return;
        baseValue = value;
        MarkDirty();
    }

    public void AddModifier(StatModifierData modifier)
    {
        modifiers.Add(modifier);
        CoroutineRunner.Instance.StartCoroutine(RemoveModifierAfterDuration(modifier));
        MarkDirty();
    }

    public void RemoveModifier(StatModifierData modifier)
    {
        modifiers.Remove(modifier);
        MarkDirty();
    }

    IEnumerator RemoveModifierAfterDuration(StatModifierData modifer)
    {
        if (modifer.useDuration && modifer.duration > 0)
        {
            yield return new WaitForSeconds(modifer.duration);
            RemoveModifier(modifer);
        }
    }

    private void MarkDirty()
    {
        isDirty = true;
        FinalValueChanged?.Invoke(FinalValue); // 최종 값이 변경되었음을 알림
    }

    private void RecalculateFinalValue()
    {
        float addSum = 0f;
        float mulSum = 1f;

        foreach (var modifier in modifiers)
        {
            if (modifier.modifierType == ModifierType.Additive)
                addSum += modifier.value;
            else if (modifier.modifierType == ModifierType.Multiplicative)
                mulSum += modifier.value;
        }

        finalValue = (baseValue + addSum) * mulSum;
    }
}
