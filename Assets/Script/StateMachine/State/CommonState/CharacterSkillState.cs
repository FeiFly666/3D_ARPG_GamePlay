using Assets.Combat;
using Assets.Model;
using Assets.StateMachine;
using Model;
using System;
using UnityEngine;

namespace Assets.Script.StateMachine.CommonState
{
    internal class CharacterSkillState<T> : IState<T> where T : CharacterBase
    {
        private T _owner;

        private StateMachine<T> _fsm;

        private SkillData skillData;
        private int animHash;

        private float lastLeftWeight;
        private float lastRightWeight;

        public void Init(T owner, StateMachine<T> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            _owner.currentState = CharacterState.Skill;

            _owner.weaponCtrl.currentWeaponObj.isSkillAttack = true;

            skillData = _owner.weaponCtrl.weaponData.weaponCombatData.skill;

            _owner.statusCtrl.DecreasePower(skillData.Cost);

            if (skillData.Type == SkillType.Action)
            {
                lastLeftWeight = _owner.anim.GetLayerWeight(1);
                lastRightWeight = _owner.anim.GetLayerWeight(2);

                _owner.anim.SetLayerWeight(1, 0);
                _owner.anim.SetLayerWeight(2, 0);

                animHash = Animator.StringToHash(skillData.animName);
                _owner.anim.CrossFadeInFixedTime(animHash, 0.1f);
            }

            if (_owner is IRestrictMove restrict)
            {
                restrict.StopMove();
            }
        }
        public void Update()
        {
            if(skillData.Type != SkillType.Action)
            {
                if(_owner is IInterruptible interruptible)
                {
                    if(interruptible.TryCancelHit())
                    {
                        return;
                    }
                }
                _owner.RevertToDefaultState();
                return;
            }

            AnimatorStateInfo stateInfo = _owner.anim.GetCurrentAnimatorStateInfo(0);
            float progress = stateInfo.normalizedTime;

            if (_owner.anim.IsInTransition(0) || stateInfo.shortNameHash != animHash)
            {
                return;
            }

            if (progress > 0.85f)
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
            _owner.anim.SetLayerWeight(1, lastLeftWeight);
            _owner.anim.SetLayerWeight(2, lastRightWeight);

            _owner.weaponCtrl.currentWeaponObj.isSkillAttack = false;

            if (_owner is IRestrictMove restrict)
            {
                restrict.StartMove();
            }
        }
    }
}
