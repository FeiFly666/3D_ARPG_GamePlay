using Assets.Combat;
using Assets.Model;
using Assets.Util;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Weapon
{
    public class WeaponObject : MonoBehaviour
    {
        private CharacterBase owner;
        //private Collider trigger;
        private List<GameObject> _HitObj = new List<GameObject>();
        public WeaponData data;

        [HideInInspector] public int currentComboDamageMult;
        [HideInInspector] public int currentComboPoiseDamage;
        [SerializeField] public float PowerUpDistanceMult = 1.5f;

        public Transform basePt; // 剑柄
        public Transform tipPt;  // 剑尖
        public float radius = 0.05f; // 刀刃的“厚度”/判定半径

        public int currentCombo = -1;

        private Vector3 _lastTipPos;
        private Vector3 _lastBasePos;

        public bool startDetect = false;
        public bool isSkillAttack;

        private void Awake()
        {
            //trigger = this.GetComponent<Collider>();   
        }
        public void SetWeaponOwner(CharacterBase character)
        {
            this.owner = character;
        }
        private static Collider[] hitColsBuffer = new Collider[100];
        private void Update()
        {
            if (!startDetect) return;

            Vector3 direction = (tipPt.position - basePt.position).normalized;

            float baseLength = Vector3.Distance(tipPt.position, basePt.position);

            float currentLength = owner.statusCtrl.IsPowerUp ? baseLength * PowerUpDistanceMult : baseLength;


            float currentRadius = owner.statusCtrl.IsPowerUp ? radius * 2.5f : radius;

            if (isSkillAttack)
            {
                direction = owner.transform.forward;
                currentLength = data.weaponCombatData.skill.attackLength;
                currentRadius = data.weaponCombatData.skill.attackWidth;
            }

            Vector3 currentTipPos = basePt.position + direction * currentLength;
            Vector3 currentBasePos = basePt.position;

            int num1 = Physics.OverlapCapsuleNonAlloc(_lastTipPos, currentTipPos, currentRadius, hitColsBuffer);

            CheckForDamage(num1);

            int num2 = Physics.OverlapCapsuleNonAlloc(currentTipPos, currentBasePos, currentRadius, hitColsBuffer);

            CheckForDamage(num2);

            _lastTipPos = currentTipPos;
            _lastBasePos = currentBasePos;
        }
        private void CheckForDamage(int num)
        {
            int PoiseDamage = currentComboPoiseDamage;

            if(owner.statusCtrl.IsPowerUp)
            {
                PoiseDamage = Mathf.RoundToInt(PoiseDamage * 1.2f);
            }

            if(isSkillAttack)
            {
                PoiseDamage *= Mathf.RoundToInt(PoiseDamage * data.weaponCombatData.skill.poiseMultiplier);
            }

            for (int i = 0; i < num; i++)
            {
                Collider other = hitColsBuffer[i];

                IDamageable damageAble = other.gameObject.GetComponent<IDamageable>();

                if (other.gameObject == owner.gameObject)
                {
                    continue;
                }

                if (other.gameObject.layer == owner.gameObject.layer) continue;

                if (damageAble != null && !_HitObj.Contains(other.gameObject))
                {
                    int damage = CombatCalculate.CalculateDamage(owner, this);

                    if(isSkillAttack)
                    {
                        damage = Mathf.RoundToInt(damage * data.weaponCombatData.skill.damageMultiplier);
                    }

                    damageAble.TakeDamage(damage, PoiseDamage, this.owner);
                    _HitObj.Add(other.gameObject);
                    if(!owner.statusCtrl.IsPowerUp)
                    {
                        owner.statusCtrl.IncreasePower(4);

                    }

                    if (owner.characterType == CharacterType.Player)
                    {
                        //if(!owner.statusCtrl.IsPowerUp)
                            HandleHitFeedback();
                    }
                }
            }
        }
        public void StartDetect()
        {
            _HitObj.Clear();
            startDetect = true;
            _lastTipPos = tipPt.position;
            _lastBasePos = basePt.position;

            owner.AllowLockRotation = false;

            if(owner.statusCtrl.IsPowerUp)
            {
                if (currentCombo != -1 && data.weaponCombatData.combatData[currentCombo] != null)
                    VFXHelper.Play(this.owner.transform, this.transform, data.weaponCombatData.combatData[currentCombo].ComboVFX);
            }

            if(isSkillAttack)
            {
                if(data.weaponCombatData.skill.Type == SkillType.Action)
                {
                    VFXHelper.Play(this.owner.transform, this.transform, data.weaponCombatData.skill.skillVFX);
                }
            }
        }

        public void StopDetect()
        {
            startDetect = false;
            owner.AllowLockRotation = true;
            //_HitObj.Clear();
        }
        private void HandleHitFeedback()
        {
            StartCoroutine(AttackFeedBack());
        }
        IEnumerator AttackFeedBack()
        {
            Time.timeScale = 0.05f;
            yield return owner.statusCtrl.IsPowerUp ? new WaitForSecondsRealtime(0.02f)  : new WaitForSecondsRealtime(0.1f);
            Time.timeScale = 1f;
        }

        private void OnDrawGizmos()
        {
            if (basePt == null || tipPt == null) return;

            // --- 1. 初始化变量 ---
            Vector3 dir;
            float currentLength;
            float currentRadius = radius;
            Vector3 currentBase = basePt.position;

            // --- 2. 核心分支逻辑（与 Update 保持高度一致） ---
            if (Application.isPlaying && isSkillAttack)
            {
                // 技能模式：方向朝前，长度取技能配置
                dir = owner.transform.forward;
                currentLength = data.weaponCombatData.skill.attackLength;
                currentRadius = data.weaponCombatData.skill.attackWidth;
            }
            else
            {
                // 普通模式：方向跟随模型，计算 PowerUp 长度
                dir = (tipPt.position - basePt.position).normalized;
                float baseLength = Vector3.Distance(tipPt.position, basePt.position);
                currentLength = (Application.isPlaying && owner != null && owner.statusCtrl.IsPowerUp)
                                ? baseLength * PowerUpDistanceMult : baseLength;
            }

            // 计算强化半径
            if (Application.isPlaying && owner != null && owner.statusCtrl.IsPowerUp)
            {
                currentRadius = radius * 2.5f;
            }

            // --- 3. 计算【动态】中心点和旋转 ---
            // 计算逻辑剑尖位置
            Vector3 virtualTip = currentBase + dir * currentLength;
            // 中心点位于手柄和逻辑剑尖的正中间
            Vector3 center = (currentBase + virtualTip) / 2f;

            // 防止 LookRotation 报错（当方向向量极小时）
            if (dir.sqrMagnitude < 0.0001f) return;
            Quaternion rotation = Quaternion.LookRotation(dir);

            // --- 4. 设置矩阵与绘制 ---
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);

            // --- 5. 设置颜色（技能显示蓝色，强化显示红色，普通显示绿色） ---
            if (Application.isPlaying && isSkillAttack)
            {
                Gizmos.color = new Color(0, 0.5f, 1, 0.5f); // 技能模式：半透明蓝色
            }
            else if (Application.isPlaying && owner != null && owner.statusCtrl.IsPowerUp)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);     // 强化模式：半透明红色
            }
            else
            {
                Gizmos.color = new Color(0, 1, 0, 0.4f);     // 普通模式：半透明绿色
            }

            // 绘制判定框 (Z 轴对应长度)
            Gizmos.DrawCube(Vector3.zero, new Vector3(currentRadius * 2, currentRadius * 2, currentLength));

            // 还原矩阵
            Gizmos.matrix = oldMatrix;

            // --- 6. 绘制轨迹 ---
            if (Application.isPlaying && startDetect)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_lastTipPos, virtualTip);
            }
        }
        /*private void OnTriggerEnter(Collider other)
        {
            IDamageable damage = other.gameObject.GetComponent<IDamageable>();

            if (other.gameObject == owner.gameObject)
            {
                return;
            }

            if (damage != null && !_HitObj.Contains(other.gameObject))
            {
                damage.TakeDamage(CombatCalculate.CalculateDamage(owner, this), currentComboPoiseDamage);
                _HitObj.Add(other.gameObject);

                if (owner.characterType == CharacterType.Player)
                {
                    HandleHitFeedback();
                }
            }
        }*/

    }
}