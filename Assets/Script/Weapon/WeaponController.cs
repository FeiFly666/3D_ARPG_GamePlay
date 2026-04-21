using Assets.Model;
using Assets.Weapon;
using Model;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private CharacterBase _owner;

    public Transform leftHandSlot, rightHandSlot;

    public WeaponObject currentWeaponObj;
    public WeaponObject currentWeaponObj2;

    public WeaponType currentWeaponType;

    public WeaponData weaponData {  get; private set; }

    public void Init(CharacterBase owner, WeaponData weapon)
    {
        this._owner = owner;

        if (rightHandSlot == null)
        {
            rightHandSlot = _owner.anim.GetBoneTransform(HumanBodyBones.RightHand);
        }

        if (leftHandSlot == null)
        {
            leftHandSlot = _owner.anim.GetBoneTransform(HumanBodyBones.LeftHand);
        }

        ChangeWeapon(weapon);
    }

    public void ChangeWeapon(WeaponData newData)
    {
        if (newData == null) return;

        weaponData = newData;

        if(currentWeaponObj != null)
        {
            Manager.Pool.ReleaseGameObject(currentWeaponObj);
        }
        if(currentWeaponObj2 != null)
        {
            Manager.Pool.ReleaseGameObject(currentWeaponObj2);
        }

        GameObject newWeaponGoPrefab = newData.weaponPrefab;

        if(newWeaponGoPrefab != null)
        {
            WeaponObject newWeaponObject = Manager.Pool.GetGameObject<WeaponObject>(newWeaponGoPrefab.GetInstanceID());

            if(newWeaponObject == null)
            {
                Manager.Pool.CreateGameObjectPool<WeaponObject>(newWeaponGoPrefab.GetComponent<WeaponObject>(), 2, 1);
                newWeaponObject = Manager.Pool.GetGameObject<WeaponObject>(newWeaponGoPrefab.GetInstanceID());
            }

            currentWeaponObj = newWeaponObject;
        }
        else
        {
            currentWeaponObj = null;
        }

        currentWeaponType = newData.weaponType;

        Transform slot1 = rightHandSlot;
        Transform slot2 = leftHandSlot;

        if(currentWeaponObj != null)
        {
            currentWeaponObj.SetWeaponOwner(this._owner);
            currentWeaponObj.transform.SetParent(slot1, false);
            currentWeaponObj.transform.localPosition = newData.positionOffset;
            currentWeaponObj.transform.localRotation = Quaternion.Euler(newData.rotationOffset.x, newData.rotationOffset.y, newData.rotationOffset.z);
            currentWeaponObj.transform.localScale = Vector3.one;
            if(_owner is EnemyController enemy)
            {
                enemy.ChangeAttackRange(newData.attackRange);
            }
        }
        if(currentWeaponType == WeaponType.OH && newData.weaponPrefab2 != null)
        {
            GameObject newWeaponGoPrefab2 = newData.weaponPrefab2;

            WeaponObject newWeaponObject = Manager.Pool.GetGameObject<WeaponObject>(newWeaponGoPrefab2.GetInstanceID());
            if(newWeaponObject != null)
            {
                newWeaponObject.SetWeaponOwner(_owner);
                newWeaponObject.transform.SetParent(slot2, false);
                newWeaponObject.transform.localPosition = newData.positionOffset2;
                newWeaponObject.transform.localRotation = Quaternion.Euler(newData.rotationOffset2.x, newData.rotationOffset2.y, newData.rotationOffset2.z);
                newWeaponObject.transform.localScale = Vector3.one;

                currentWeaponObj2 = newWeaponObject;
            }
        }
        
        RuntimeAnimatorController animatorOverride = _owner.characterSex == CharacterSex.Female ? newData.weaponCombatData.femaleAnimatorOverride : newData.weaponCombatData.maleAnimatorOverride;
        if (animatorOverride != null)
        {
            _owner.anim.runtimeAnimatorController = animatorOverride;
        }

        if(_owner.statusCtrl.IsPowerUp)
        {
            _owner.statusCtrl.PowerUpEnd();
        }

        _owner.anim.SetFloat("WeaponType", (int)currentWeaponType);

        //float leftWeight = currentWeaponType == WeaponType.TH ? 1f : 0f;
        float leftWeight = currentWeaponType == WeaponType.TH ? 1f : 0f;
        float rightWeight = currentWeaponType == WeaponType.TH ? 1f : 0f;

        _owner.ChangeUpBodyWeight(leftWeight, rightWeight);

    }

}
