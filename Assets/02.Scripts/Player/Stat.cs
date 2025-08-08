using System.Collections.Generic;
using UnityEngine;
using Constants;
using System;

[System.Serializable]
public class Stat
{
    public event Action<float, float> BaseValueChanged;

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
        MarkDirty();
    }

    public void RemoveModifier(StatModifierData modifier)
    {
        modifiers.Remove(modifier);
        MarkDirty();
    }
    /*
    public void Update(float deltaTime)
    {
        bool removed = false;

        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            var m = modifiers[i];
            if (m.duration > 0) // 무한 지속은 0 이하로
            {
                m.duration -= deltaTime;
                if (m.duration <= 0)
                {
                    modifiers.RemoveAt(i);
                    removed = true;
                }
            }
        }

        if (removed)
            MarkDirty();
    }
    */

    private void MarkDirty() => isDirty = true;

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

        // mulSum이 1이면 100% 증가 = x2
        finalValue = (baseValue + addSum) * mulSum;
    }
}
