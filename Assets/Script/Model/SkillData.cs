using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Model
{
    public enum SkillType
    {
        Buff,
        Action,
        TianXiaWuShuang
    }
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Combat/SkillData")]
    public class SkillData : ScriptableObject
    {
        public string Name;

        public string Description;

        public SkillType Type;

        [Header("用于动作技能配置")]

        public string animName;

        public EffectData skillVFX;

        public float Cost = 50f;

        public float attackLength = 10f;
        public float attackWidth = 5f;

        public float damageMultiplier = 2.0f; // 伤害倍率
        public float poiseMultiplier = 2.0f;  // 削韧倍率

        [Header("用于Buff配置")]

        //public buff类型 还没做先空着

        public float duration = 10f;

        public float multiplier = 1f;

    }
}
