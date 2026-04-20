using Assets.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Weapon Combat", menuName = "Combat/WeaponCombatData")]
public class WeaponCombatData : ScriptableObject
{
    //public int comboCount;
    [Header("武器数值")]
    public int weaponDamage = 5;

    [Header("技能")]

    public SkillData skill;

    [Header("动作表现")]
    public RuntimeAnimatorController femaleAnimatorOverride;
    public RuntimeAnimatorController maleAnimatorOverride;

    [Header("连招表现")]
    public EffectData ParrayVFX;
    public CombatData[] combatData;
}

[Serializable]
public class CombatData
{
    public float cancelThreshold;//每段连招的后摇
    public int comboPoiseDamage;//每段连招的削韧
    public int comboStaminaCost;//每段连招的体力消耗

    public EffectData ComboVFX;
}
