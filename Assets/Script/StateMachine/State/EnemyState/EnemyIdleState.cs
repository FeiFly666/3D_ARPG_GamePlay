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

            _owner.DisableAgent();

        }
        public void Update()
        {
            
            if(_owner.IsDetectPlayer())
            {
                //TODO:追击玩家
                _fsm.ChangeState<EnemyChaseState>();
                if (_owner.characterType == CharacterType.Boss)
                {
                    Manager.Event.Execute(EventManager.Event_Type.Boss_Find_Player, this._owner);
                    _owner.statusCtrl.InvokeUIChange();
                }
                return;
            }

        }
       
        public void Exit()
        {
            
        }
    }
}
