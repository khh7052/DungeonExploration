using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : Singleton<InputManager>
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool FireInput { get; private set; }
    public bool DashInput { get; private set; }
    public int SelectInput { get; private set; }
    public float ZoomInput { get; private set; }

    public event Action InteractAction;
    public event Action<int> SelectAction;
    public event Action DropAction;
    public event Action UseAction;
    public event Action OptionAction;

    public bool IsMoving => MoveInput.sqrMagnitude > 0.01f;

    public void OnMove(InputAction.CallbackContext callback)
    {
        if (callback.performed)
            MoveInput = callback.ReadValue<Vector2>();
        else if (callback.canceled)
            MoveInput = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext callback)
    {
        if (callback.performed)
            LookInput = callback.ReadValue<Vector2>();
        else if (callback.canceled)
            LookInput = Vector2.zero;
    }

    public void OnJump(InputAction.CallbackContext callback)
    {
        if (callback.performed)
            JumpInput = true;
        else if (callback.canceled)
            JumpInput = false;
    }

    public void OnFire(InputAction.CallbackContext callback)
    {
        if (callback.performed)
            FireInput = true;
        else if (callback.canceled)
            FireInput = false;
    }

    public void OnDash(InputAction.CallbackContext callback)
    {
        if (callback.performed)
            DashInput = true;
        else if (callback.canceled)
            DashInput = false;
    }

    public void OnInteract(InputAction.CallbackContext callback)
    {
        if (callback.started)
            InteractAction?.Invoke();
    }

    public void OnSelect(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            var keyControl = callback.control as KeyControl;
            if (keyControl != null)
            {
                string keyName = keyControl.keyCode.ToString();
                if (keyName.StartsWith("Digit"))
                {
                    SelectInput = keyName[^1] - '0';
                    SelectAction?.Invoke(SelectInput);
                }
            }
        }
    }

    public void OnOption(InputAction.CallbackContext callback)
    {
        if (callback.started)
            OptionAction?.Invoke();
    }

    public void OnDrop(InputAction.CallbackContext callback)
    {
        if (callback.started)
            DropAction?.Invoke();
    }

    public void OnUse(InputAction.CallbackContext callback)
    {
        if (callback.started)
            UseAction?.Invoke();
    }

    public void OnZoom(InputAction.CallbackContext callback)
    {
        if (callback.performed)
            ZoomInput = callback.ReadValue<Vector2>().y / 120f;
        else if (callback.canceled)
            ZoomInput = 0f;
    }

}
