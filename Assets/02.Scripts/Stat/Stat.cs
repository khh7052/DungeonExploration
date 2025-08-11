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
    private bool isDirty = true; // ���� ���� ����Ǿ����� ����

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
        finalValue = baseValue; // �ʱ� ���� ���� �⺻ ���� ����
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
        FinalValueChanged?.Invoke(FinalValue); // ���� ���� ����Ǿ����� �˸�
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
