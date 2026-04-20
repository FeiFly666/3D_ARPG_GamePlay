using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.StateMachine.EnemyState
{
    public class EnemyIdleState : IState<EnemyController>
    {
        private EnemyController _owner;
        private StateMachine<EnemyController> _fsm;

        private float _targetSearchTimer = 0;
        public void Init(EnemyController owner, StateMachine<EnemyController> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            Debug.Log($"{_owner.gameObject.name} 进入待机状态");
            if(_owner.characterType == CharacterType.Boss)
            {
                Manager.Event.Execute(EventManager.Event_Type.Boss_Lose_Player);
            }

            _owner.currentState = CharacterState.Idle;

            string nextAnim = _owner.anim.GetBool("IsLockOn") ? "LockOnForwardWalk" : "Walk";
            _owner.anim.CrossFadeInFixedTime(nextAnim, 0.15f);

            _targetSearchTimer = 0;

            _owner.InputMove = Vector2.zero;

            _owner.DisableAgent();

        }
        public void Update()
        {
            if(_owner.IsDetectTarget())
            {
                _fsm.ChangeState<EnemyChaseState>();
                if (_owner.characterType == CharacterType.Boss)
                {
                    Manager.Event.Execute(EventManager.Event_Type.Boss_Find_Player, this._owner);
                    _owner.statusCtrl.InvokeUIChange();
                }
                return;
            }
            else
            {
                _targetSearchTimer -= Time.deltaTime;
                if (_targetSearchTimer < 0)
                {
                    _targetSearchTimer = 0.2f;
                    CharacterBase newTarget = _owner.targetSystem.FindBestTarget();
                    if(newTarget != null)
                    {
                        _owner.SetLockedTarget(newTarget);
                    }
                }
            }

        }
       
        public void Exit()
        {
            
        }
    }
}
