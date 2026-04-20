using Assets.Combat;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.StateMachine.CommonState
{
    public class CharacterDeathState<T> : IState<T> where T : CharacterBase
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
            _owner.currentState = CharacterState.Dead;
            _owner.anim.SetTrigger("Death");

            _owner.AllowLockRotation = false;

            if (_owner is IRestrictMove restrict)
            {
                restrict.StopMove();
            }
        }
        public void Update()
        {
            AnimatorStateInfo stateInfo = _owner.anim.GetCurrentAnimatorStateInfo(0);
            float progress = stateInfo.normalizedTime;

            if (_owner.anim.IsInTransition(0) || !stateInfo.IsName("Death"))
            {
                return;
            }

            if (progress >= 1f)
            {
                _owner.RevertToDefaultState();
            }
        }
        public void Exit()
        {
            GameObject.Destroy(_owner.gameObject);

            if(_owner.characterType == CharacterType.Boss)
            {
                Manager.Event.Execute(EventManager.Event_Type.Boss_Lose_Player);
            }
            /*_owner.AllowLockRotation = true;

            if (_owner is IRestrictMove restrict)
            {
                restrict.StartMove();
            }*/
        }
    }
}