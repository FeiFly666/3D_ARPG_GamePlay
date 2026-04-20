using Assets.Combat;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.StateMachine.CommonState
{
    public class CharacterStaggerState<T> :IState<T> where T : CharacterBase
    {
        private T _owner;

        private StateMachine<T> _fsm;

        public void Init(T owner, StateMachine<T> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            _owner.currentState = CharacterState.Hit;
            _owner.anim.CrossFadeInFixedTime("Stagger", 0.2f);
            _owner.AllowLockRotation = false;
            _owner.statusCtrl.isStagger = true;

            if (_owner is IRestrictMove restrict)
            {
                restrict.StopMove();
            }
        }
        public void Update()
        {
            AnimatorStateInfo stateInfo = _owner.anim.GetCurrentAnimatorStateInfo(0);
            float progress = stateInfo.normalizedTime;

            if (_owner.anim.IsInTransition(0) || !stateInfo.IsName("Stagger"))
            {
                return;
            }

            if (progress > 0.8f)
            {
                if (_owner is IInterruptible interruptible)
                {
                    if (interruptible.TryCancelHit())
                        return;
                }
            }

            if (progress >= 1f)
            {
                _owner.RevertToDefaultState();
            }
        }
        public void Exit()
        {
            _owner.statusCtrl.isStagger = false;
            _owner.AllowLockRotation = true;

            if (_owner is IRestrictMove restrict)
            {
                restrict.StartMove();
            }
        }
    }
}
