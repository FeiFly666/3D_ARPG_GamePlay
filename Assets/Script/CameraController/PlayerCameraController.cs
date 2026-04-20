using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
public enum CamState
{ 
    Free,
    Lock
}

public class PlayerCameraController : MonoBehaviour
{
    [Header("相机")]
    public CinemachineFreeLook freeCam;
    public CinemachineVirtualCamera lockCam;
    [Header("相机优先级")]
    [SerializeField] private int positive;
    [SerializeField] private int negative;

    [Header("锁定物体")]
    public CinemachineTargetGroup lockTarget;
    public Transform lockPivot;

    public Transform lockfollow;

    Vector3 pivotVel = default;
    Vector3 followPosVel = default;

    [Header("--------")]
    public Transform playerCharacter;

    public Transform lockedEnemy;

    public CamState camState;

/*    float switchSmooth = 6f;
    float followSmooth = 25f;*/

    public bool isSwitching = false;

    private void Awake()
    {
        SetPlayerCharacter(this.transform);

        GameObject lockTargetGroupGo = new GameObject("LockTargetGroup");
        this.lockTarget = lockTargetGroupGo.AddComponent<CinemachineTargetGroup>();

        GameObject lockFollowGo = new GameObject("LockFollow");
        this.lockfollow = lockFollowGo.transform;

        UpdateLockFollowPosition();

        GameObject lockPivotGo = new GameObject("LockPivot");

        lockPivot = lockPivotGo.transform;

        this.lockCam.LookAt = lockPivot;
        this.lockCam.Follow = lockfollow;

        StopLockOn();
    }

    public void SetPlayerCharacter(Transform playerCharacter)
    {
        this.playerCharacter = playerCharacter.Find("LookPoint");

        freeCam.LookAt = this.playerCharacter;
        freeCam.Follow = playerCharacter;
    }
    public void SetLockedEnemy(Transform lockedEnemy)
    {
        if (FindMember(this.lockedEnemy) >= 0)
        {
            lockTarget.RemoveMember(this.lockedEnemy);
        }

        this.lockedEnemy = lockedEnemy;

        if (FindMember(playerCharacter) < 0)
            lockTarget.AddMember(this.playerCharacter, 1f, 1);

        if(!HasMember(lockedEnemy))
            lockTarget.AddMember(lockedEnemy, 1, 1);

        pivotVel = Vector3.zero;
    }
    private int FindMember(Transform transform)
    {
        for (int i = 0; i < lockTarget.m_Targets.Length; i++)
        {
            if (lockTarget.m_Targets[i].target == transform)
            {
                return i;
            }
        }
        return -1;
    }
    bool HasMember(Transform t)
    {
        for (int i = 0; i < lockTarget.m_Targets.Length; i++)
        {
            if (lockTarget.m_Targets[i].target == t)
                return true;
        }
        return false;
    }
    public void StartLockOn()
    {
        lockCam.Priority = positive;
        freeCam.Priority = negative;

        //lockTarget.m_Targets = new CinemachineTargetGroup.Target[0];

        camState = CamState.Lock;
    }

    public void StopLockOn()
    {
        // 切换
        freeCam.Priority = positive;
        lockCam.Priority = negative;

        freeCam.ForceCameraPosition(lockCam.transform.position, lockCam.transform.rotation);

        lockTarget.m_Targets = new CinemachineTargetGroup.Target[0];
        camState = CamState.Free;
    }
    private void LateUpdate()
    {
        if (camState == CamState.Lock && lockTarget.m_Targets.Length > 0)
        {
            Vector3 center = lockTarget.transform.position;

            float smoothTime = isSwitching ? 0.3f : 0.02f;

            lockPivot.position = Vector3.SmoothDamp(lockPivot.position, center, ref pivotVel, smoothTime);

            if (isSwitching && (lockPivot.position - center).sqrMagnitude < 0.0025f)
            {
                isSwitching = false;
            }

            UpdateLockFollowPosition();
        }
    }
    private void UpdateLockFollowPosition()
    {
        Vector3 playerCurrentPos = this.transform.position;

        playerCurrentPos.y += 1.2f;

        //lockfollow.transform.position = playerCurrentPos;
        lockfollow.position = Vector3.SmoothDamp(lockfollow.position, playerCurrentPos, ref followPosVel, 0.05f);

        Vector3 dir = lockTarget.transform.position - lockfollow.position;
        dir.y = 0; // 保持水平视角

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetWorldRot = Quaternion.LookRotation(dir);

            //lockfollow.rotation = targetWorldRot;
            float rotationSpeed = isSwitching ? 5f : 20f;
            lockfollow.rotation = Quaternion.Slerp(lockfollow.rotation, targetWorldRot, rotationSpeed * Time.deltaTime);
        }
    }
}
