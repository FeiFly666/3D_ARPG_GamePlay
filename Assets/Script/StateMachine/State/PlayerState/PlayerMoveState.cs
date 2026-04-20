using System;
using Assets.StateMachine.CommonState;
using Model;
using UnityEngine;

namespace Assets.StateMachine.PlayerState
{
    public class PlayerMoveState : IState<PlayerController>
    {
        private PlayerController _owner;

        private StateMachine<PlayerController> _fsm;

        public void Init(PlayerController owner, StateMachine<PlayerController> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            _owner.currentState = CharacterState.Move;

            string nextAnim = _owner.anim.GetBool("IsLockOn") ? "LockOnForwardWalk" : "Walk";
            _owner.anim.CrossFadeInFixedTime(nextAnim, 0.15f);
        }
        public void Update()
        {
            if(_owner.InputMove == Vector2.zero)
            {
                _fsm.ChangeState<PlayerIdleState>();
                return;
            }
            if (_owner.InputAttack && _owner.weaponCtrl.currentWeaponType != WeaponType.None)
            {
                if(_owner.statusCtrl.CanUseStamina(_owner.weaponCtrl.weaponData.weaponCombatData.combatData[0].comboStaminaCost))
                {
                    _fsm.ChangeState<PlayerAttackState>();
                    return;
                }
            }

            if (_owner.InputBlocking)
            {
                if(_owner.weaponCtrl.currentWeaponType != WeaponType.None && _owner.statusCtrl.currentStamina >= _owner.statusCtrl.maxStamina * 0.05f)
                {
                    _fsm.ChangeState<CharacterBlockState<PlayerController>>();
                    return;
                }
            }


            _owner.UseSkillLogic();
            _owner.AddMoveLogic();
            _owner.WeaponChangeLogic();
        }
        public void Exit()
        {

        }
    }
}
