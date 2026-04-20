using Assets.Combat;
using Assets.StateMachine.PlayerState;
using Model;
using System;
using UnityEngine;

namespace Assets.StateMachine.CommonState
{
    public class CharacterBlockState<T> : IState<T> where T : CharacterBase
    {
        private T _owner;

        private StateMachine<T> _fsm;

        private float enterTimer = 0f;

        private float lastLeftWeight;
        private float lastRightWeight;


        public void Init(T owner, StateMachine<T> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            _owner.currentState = CharacterState.Block;
            _owner.anim.CrossFadeInFixedTime("Block", 0.1f);

            enterTimer = 0f;

            lastLeftWeight = _owner.anim.GetLayerWeight(1);
            lastRightWeight = _owner.anim.GetLayerWeight(2);

            _owner.anim.SetLayerWeight(1, 0);
            _owner.anim.SetLayerWeight(2, 0);

            //_owner.AllowLockRotation = false;
            _owner.statusCtrl.CanRecoverPoise = false;
            _owner.statusCtrl.CanRecoverStamina = false;

            _owner.statusCtrl.IsBlocking = true;
            _owner.statusCtrl.IsPerfectBlock = false;

            if(_owner is IRestrictMove restrict)
            {
                restrict.StopMove();
            }
        }
        public void Update()
        {
            enterTimer += Time.deltaTime;
            if(enterTimer <= 0.08f)
            {
                return;
            }
            if(enterTimer > 0.08f)
            {
                _owner.statusCtrl.IsPerfectBlock = true;
            }
            if (_owner.statusCtrl.IsPowerUp)
            {
                if (enterTimer > 0.25f)
                {
                    _owner.statusCtrl.IsPerfectBlock = false;
                }
            }
            else
            {
                if (enterTimer > 0.16f)
                {
                    _owner.statusCtrl.IsPerfectBlock = false;
                }
            }

            _owner.statusCtrl.UseStamina(0);


            if (_owner.statusCtrl.blockedPerfectly && _owner is ICounterable counterable)
            {
                if (counterable.TryExecuteCounter())
                {
                    return;
                }
            }

            AnimatorStateInfo stateInfo = _owner.anim.GetCurrentAnimatorStateInfo(0);

            if (_owner.anim.IsInTransition(0) || !stateInfo.IsName("Block")) return;

            if (!_owner.statusCtrl.blockedPerfectly)
                _owner.statusCtrl.UseStamina(5 * Time.deltaTime);


            if (_owner.IsBlockReleased() || !_owner.statusCtrl.blockedPerfectly && !_owner.statusCtrl.CanUseStamina(5 * Time.deltaTime))
            {

                if(_owner is IInterruptible interruptible && interruptible.TryCancelHit())
                {
                    return;
                }
                _owner.RevertToDefaultState();
            }
        }
        public void Exit()
        {
            _owner.statusCtrl.IsBlocking = false;
            _owner.statusCtrl.IsPerfectBlock = false;

            _owner.anim.SetLayerWeight(1, lastLeftWeight);
            _owner.anim.SetLayerWeight(2, lastRightWeight);

            _owner.statusCtrl.CanRecoverPoise = true;
            _owner.statusCtrl.CanRecoverStamina = true;

            //_owner.AllowLockRotation = true;

            if (_owner is IRestrictMove restrict)
            {
                restrict.StartMove();
            }
        }

    }
}
