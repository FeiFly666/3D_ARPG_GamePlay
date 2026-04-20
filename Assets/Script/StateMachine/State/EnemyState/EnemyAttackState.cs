using Assets.StateMachine.PlayerState;
using Assets.Util;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

namespace Assets.StateMachine.EnemyState
{
    public class EnemyAttackState : IState<EnemyController>
    {
        private EnemyController _owner;

        private float lastLeftWeight;
        private float lastRightWeight;

        private StateMachine<EnemyController> _fsm;

        private int thisAttackComboNum;
        private int comboNum = 0;

        private int currentCombo = 1;
        private float currentAttackInterval = 0f;
        //private float lastAttackTime = 0;

        public void Init(EnemyController owner, StateMachine<EnemyController> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            //_owner.DisableAgent();

            _owner.currentState = CharacterState.Attack;

            currentCombo = -1;

            lastLeftWeight = _owner.anim.GetLayerWeight(1);
            lastRightWeight = _owner.anim.GetLayerWeight(2);

            _owner.anim.SetLayerWeight(1, 0);
            _owner.anim.SetLayerWeight(2, 0);

            _owner.statusCtrl.CanRecoverStamina = false;

            this.thisAttackComboNum = Random.Range(3, _owner.weaponCtrl.weaponData.weaponCombatData.combatData.Length);
            this.comboNum = 0;

            PerformNextCommbo();

            _owner.StopMove();
            
        }
        public void Update()
        {
            AnimatorStateInfo stateInfo = _owner.anim.GetCurrentAnimatorStateInfo(0);
            float progress = stateInfo.normalizedTime;

            if (_owner.anim.IsInTransition(0) || !stateInfo.IsName("Attack" + currentCombo)) return;

            if (progress > _owner.weaponCtrl.weaponData.weaponCombatData.combatData[currentCombo].cancelThreshold + currentAttackInterval)
            {
                if(!CombatCalculate.IsInRange(_owner.DisToTarget(), _owner.GetAttackRange() * 1.01f))
                {
                    _fsm.ChangeState<EnemyCombatState>();
                    return;
                }

                comboNum++;
                if (comboNum >= thisAttackComboNum)
                {
                    _fsm.ChangeState<EnemyCombatState>();
                    return;
                }
                PerformNextCommbo();
            }
/*            if (Time.time - lastAttackTime > 3f)
            {
                PerformNextCommbo();
                comboNum++;
                if (comboNum >= thisAttackComboNum)
                {
                    _fsm.ChangeState<EnemyCombatState>();
                    return;
                }

                lastAttackTime = Time.time;
            }*/

        }
        private bool PerformNextCommbo()
        {
            int nextCombo = (Random.Range(0,100)) % (_owner.weaponCtrl.weaponData.weaponCombatData.combatData.Count());

            /*if (_owner.statusCtrl.UseStamina(_owner.weaponCtrl.weaponData.comboStaminaCost[nextCombo]))
            {
                hasInputInThisCombo = true;
                currentCombo = nextCombo;

                Debug.Log($"当前连段：{currentCombo}");

                PlayAttackAnim();

                return true;
            }

            Debug.Log($"{_owner.gameObject.name}余力不足以攻击");

            return false;*/

            currentCombo = nextCombo;
            currentAttackInterval = Random.Range(0f, 0.10f);

            _owner.weaponCtrl.currentWeaponObj.currentComboDamageMult = 0;
            _owner.weaponCtrl.currentWeaponObj.currentComboPoiseDamage = _owner.weaponCtrl.weaponData.weaponCombatData.combatData[currentCombo].comboPoiseDamage;

            _owner.weaponCtrl.currentWeaponObj.currentCombo = nextCombo;

            //Debug.Log($"当前连段：{currentCombo}");

            PlayAttackAnim();
            return true;
        }
        private void PlayAttackAnim()
        {
            _owner.anim.CrossFadeInFixedTime("Attack" + currentCombo, 0.2f);
        }
        public void Exit()
        {
            Debug.Log($"{_owner.gameObject.name}退出攻击状态");
            _owner.anim.SetLayerWeight(1, lastLeftWeight);
            _owner.anim.SetLayerWeight(2, lastRightWeight);

            _owner.statusCtrl.CanRecoverStamina = true;

            _owner.StartMove();
        }
    }
}
