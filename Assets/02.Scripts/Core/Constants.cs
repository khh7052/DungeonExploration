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

    public enum SoundType
    {
        BGM,
        SFX,
    }

    public static class AnimatorHash
    {
        // Player
        public static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        public static readonly int JumpHash = Animator.StringToHash("Jump");
        public static readonly int DashHash = Animator.StringToHash("Dash");
        public static readonly int ClimbHash = Animator.StringToHash("Climb");

        // Platform Launcher
        public static readonly int LaunchReadyHash = Animator.StringToHash("LaunchReady");
        public static readonly int LaunchHash = Animator.StringToHash("Launch");

        // Jump Pad
        public static readonly int BounceHash = Animator.StringToHash("Bounce");

    }

}
