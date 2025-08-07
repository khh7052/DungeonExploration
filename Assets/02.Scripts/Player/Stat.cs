using Constants;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    private Action<float, float> BaseValueChanged;

    [SerializeField] private StatType type;
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifierData> modifiers = new();

    public StatType Type { get => type; }
    public float BaseValue { get => baseValue; private set => baseValue = value; }
    public float FinalValue
    {
        get
        {
            float addSum = 0f;
            float mulSum = 1f;

            foreach (var modifier in modifiers)
            {
                if (modifier.modifierType == ModifierType.Additive)
                    addSum += modifier.value;
                else if (modifier.modifierType == ModifierType.Multiplicative)
                    mulSum *= modifier.value;
            }

            return (baseValue + addSum) * mulSum;
        }
    }

    public Stat(StatType type, float baseValue)
    {
        this.type = type;
        this.baseValue = baseValue;
    }

    public void SetBaseValue(float value)
    {
        if (Mathf.Approximately(baseValue, value)) return;

        baseValue = value;
        BaseValueChanged?.Invoke(BaseValue, FinalValue);
    }

    public void AddModifier(StatModifierData modifier) => modifiers.Add(modifier);
    public void RemoveModifier(StatModifierData modifier) => modifiers.Remove(modifier);
    public void ClearModifiers() => modifiers.Clear();

    public void RegisterBaseValueChanged(Action<float, float> callback) => BaseValueChanged += callback;
    public void UnregisterBaseValueChanged(Action<float, float> callback) => BaseValueChanged -= callback;
}
