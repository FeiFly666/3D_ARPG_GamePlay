using Assets.StateMachine.CommonState;
using Model;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.StateMachine.PlayerState
{
    internal class PlayerAttackState :IState<PlayerController>
    {
        private PlayerController _owner;

        private float lastLeftWeight;
        private float lastRightWeight;

        private StateMachine<PlayerController> _fsm;

        private bool input = true;
        private bool hasInputInThisCombo = false;

        private int currentCombo = 1;

        public void Init(PlayerController owner, StateMachine<PlayerController> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            _owner.currentState = CharacterState.Attack;

            input = true;
            hasInputInThisCombo = false;
            currentCombo = -1;

            lastLeftWeight = _owner.anim.GetLayerWeight(1);
            lastRightWeight = _owner.anim.GetLayerWeight(2);

            _owner.anim.SetLayerWeight(1, 0);
            _owner.anim.SetLayerWeight(2, 0);

            _owner.statusCtrl.CanRecoverStamina = false;
            _owner.statusCtrl.CanRecoverPoise = false;
            //_owner.AllowLockRotation = false;

            if(!PerformNextCommbo())
            {
                if (_owner.InputMove != Vector2.zero)
                {
                    _fsm.ChangeState<PlayerMoveState>();
                }
                else
                {
                    _fsm.ChangeState<PlayerIdleState>();
                }
            }
        }
        public void Update()
        {
            AnimatorStateInfo stateInfo = _owner.anim.GetCurrentAnimatorStateInfo(0);
            float progress = stateInfo.normalizedTime;

            if (_owner.anim.IsInTransition(0) || !stateInfo.IsName("Attack" + currentCombo)) return;

            if (_owner.InputAttack)
            {
                input = true;
                _owner.InputAttack = false;
            }
            if(progress <= 0.7f)
            {
                hasInputInThisCombo = false;
            }
            if(progress > 0.7f && progress < 0.95f)
            {
                if(input && !hasInputInThisCombo)
                {
                    PerformNextCommbo();
                }
            }
            if(progress > _owner.weaponCtrl.weaponData.weaponCombatData.combatData[currentCombo].cancelThreshold)
            {
                if(_owner.InputMove != Vector2.zero)
                {
                    _fsm.ChangeState<PlayerMoveState>();
                    return;
                }
                if(_owner.InputBlocking && _owner.statusCtrl.currentStamina >= _owner.statusCtrl.maxStamina * 0.1f)
                {
                    _fsm.ChangeState<CharacterBlockState<PlayerController>>();
                    return;
                }
            }
            if (progress >= 1f)
            {
                _fsm.ChangeState<PlayerIdleState>();
            }
            input = false;

        }
        private bool PerformNextCommbo()
        {
            input = false;

            int nextCombo = (currentCombo + 1) % (_owner.weaponCtrl.weaponData.weaponCombatData.combatData.Count());

            if (_owner.statusCtrl.UseStamina(_owner.weaponCtrl.weaponData.weaponCombatData.combatData[nextCombo].comboStaminaCost))
            {
                hasInputInThisCombo = true;
                currentCombo = nextCombo;

                _owner.weaponCtrl.currentWeaponObj.currentComboDamageMult = 0;
                _owner.weaponCtrl.currentWeaponObj.currentComboPoiseDamage = _owner.weaponCtrl.weaponData.weaponCombatData.combatData[currentCombo].comboPoiseDamage;

                _owner.weaponCtrl.currentWeaponObj.currentCombo = this.currentCombo;
                //Debug.Log($"当前连段：{currentCombo}");

                PlayAttackAnim();

                return true;
            }

            //Debug.Log($"{_owner.gameObject.name}余力不足以攻击");

            return false;
        }
        private void PlayAttackAnim()
        {
            _owner.anim.CrossFadeInFixedTime("Attack" + currentCombo, 0.2f);
        }
        public void Exit()
        {
            Debug.Log($"{_owner.gameObject.name}退出攻击状态");

            if(_owner.weaponCtrl.currentWeaponObj != null)
                _owner.weaponCtrl.currentWeaponObj.currentCombo = -1;

            _owner.anim.SetLayerWeight(1, lastLeftWeight);
            _owner.anim.SetLayerWeight(2, lastRightWeight);

            _owner.statusCtrl.CanRecoverStamina = true;
            _owner.statusCtrl.CanRecoverPoise = true;

            //_owner.AllowLockRotation = true;
        }
    }
}
