using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public enum UIType
    {
        None,
        MainMenu,
        HUD,
        Settings,
        GameOver,
        Inventory,
        Dialogue
    }

    public enum StatType
    {
        MaxHP,
        CurrentHP,
        Attack,
        Defense,
        MoveSpeed,
        JumpForce,
        DashDistance,
    }
    public enum ModifierType
    {
        Additive,     // +5
        Multiplicative, // x1.2
    }


}
