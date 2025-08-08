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
        DashForce,
        DashDuration
    }
    public enum ModifierType
    {
        Additive,     // +5
        Multiplicative, // x1.2
    }

    public static class AnimatorHash
    {
        public static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        public static readonly int JumpHash = Animator.StringToHash("Jump");
    }

}
