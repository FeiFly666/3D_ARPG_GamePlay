using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class LocalPlayerBridge : MonoBehaviour
{
    PlayerController controller;
    PlayerInputHandler input;

    PlayerCameraController cameraController;
    TargetSystem targetSystem;

    private bool _canSwitch = true;
    private float _mouseTimer = 0;


    void Awake()
    {
        controller = GetComponent<PlayerController>();
        input = GetComponent<PlayerInputHandler>();
        cameraController = GetComponent<PlayerCameraController>();
        targetSystem = GetComponent<TargetSystem>();
    }
    private void Start()
    {
        cameraController.SetPlayerCharacter(this.transform); 
    }

    // Update is called once per frame
    void Update()
    {
        controller.SetInput(input.Move, input.IsRunning, input.IsChangeWeapon, input.IsAttack, input.IsBlocking, input.IsPowerUp);

        if(input.IsLockOnPressed)
        {
            if(!controller.IsLockOn)
            {
                if(HandleLockOnTarget())
                {
                    cameraController.StartLockOn();
                    controller.IsLockOn = true;
                }
            }
            else
            {
                cameraController.StopLockOn();

                controller.UnLockTarget();

                targetSystem.ClearTarget();

                controller.IsLockOn = false;
            }
        }

        if (controller.IsLockOn)
        {
            //controller.RotateToLockTarget();
            if(HandleTargetSwitch())
            {
                controller.StartSwitching();
                cameraController.isSwitching = true;
            }
        }
    }
    bool HandleLockOnTarget()
    {
        Transform newTarget = targetSystem.FindBestTarget();



        if (newTarget != null)
        {
            controller.SetLockedTarget(newTarget);
            cameraController.SetLockedEnemy(newTarget);

            return true;
        }

        return false;
    }
    bool HandleTargetSwitch()
    {
        float switchInput = input.Look.x;

        float threshold = input.IsGamepad ? 0.7f : 50f;

        if (Mathf.Abs(switchInput) > threshold && _canSwitch)
        {
            Transform next = targetSystem.GetNextTarget(switchInput);

            _canSwitch = false;

            if (!input.IsGamepad) _mouseTimer = 0.05f;

            if (next != null)
            {
                controller.SetLockedTarget(next);
                cameraController.SetLockedEnemy(next);

                return true;
            }
        }
        else if (input.IsGamepad &&Mathf.Abs(switchInput) < 0.1f)
        {
            _canSwitch = true;
        }
        else if(!input.IsGamepad)
        {
            if (_mouseTimer > 0) _mouseTimer -= Time.deltaTime;
            else _canSwitch = true;
        }

        return false;
    }

}
