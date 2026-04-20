using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerInputActions input;
    public bool IsGamepad {  get; private set; }
    public Vector2 Move {  get; private set; }
    public Vector2 Look { get; private set; }
    public bool IsRunning {  get; private set; }

    public bool IsLockOnPressed { get; private set; }

    bool lockOnPressed = false;

    public bool IsChangeWeapon {  get; private set; }
    bool changeWeaponPressed = false;

    public bool IsAttack { get; private set; }
    public bool IsBlocking { get; private set; }

    public bool IsPowerUp { get; private set; }

    private void Awake()
    {
        input  = new PlayerInputActions();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;

        input.Enable();
        //Debug.Log(Gamepad.current);
    }
    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;

        input.Disable();
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted || change == InputActionChange.ActionPerformed)
        {
            var action = (InputAction)obj;

            if (action.activeControl.device is Gamepad)
            {
                IsGamepad = true;
            }
            else if (action.activeControl.device is Mouse)
            {
                IsGamepad = false;
            }
        }
    }

    private void Update()
    {
        Move = input.Player.Move.ReadValue<Vector2>();

        Vector2 newLook = input.Player.Look.ReadValue<Vector2>();
        newLook.x *= -1;
        newLook.y *= -1;

        Look = newLook;

        IsRunning = input.Player.Run.IsPressed();
        
        IsAttack = input.Player.Attack.IsPressed();

        IsBlocking = input.Player.Block.IsPressed();

        IsPowerUp = input.Player.PowerUp.IsPressed();

        if(input.Player.LockOn.IsPressed() && !lockOnPressed )
        {
            lockOnPressed = true;
            IsLockOnPressed = true;
        }
        else
        {
            IsLockOnPressed = false;
            if (!input.Player.LockOn.IsPressed())
                lockOnPressed = false;
        }

        if(input.Player.WeaponChange.IsPressed() && !changeWeaponPressed )
        {
            changeWeaponPressed = true;
            IsChangeWeapon = true;
        }
        else if(!input.Player.WeaponChange.IsPressed())
        {
            changeWeaponPressed = false;
            IsChangeWeapon = false;
        }
    }
}
