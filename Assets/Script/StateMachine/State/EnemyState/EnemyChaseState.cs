using Model;
using System;
using UnityEngine;

namespace Assets.StateMachine.EnemyState
{
    public class EnemyChaseState : IState<EnemyController>
    {
        private EnemyController _owner;
        private StateMachine<EnemyController> _fsm;
        private float _pathUpdateTimer;

        public void Init(EnemyController owner, StateMachine<EnemyController> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            Debug.Log($"{_owner.gameObject}进入追击状态");

            _owner.currentState = CharacterState.Move;

            _owner.IsLockOn = false;

            //_owner.agent.isStopped = false; 

            _owner.agent.updateRotation = true;

            _owner.EnableAgent();
        }
        public void Update()
        {
            //_owner.agent.nextPosition = _owner.transform.position;

            if(_owner.lockedTarget == null)
            {
                _fsm.ChangeState<EnemyIdleState>();
                return;
            }

            float dist = _owner.DisToTarget();

            if (dist <= 16f)
            {
                _fsm.ChangeState<EnemyCombatState>();
                return;
            }

            // --- 导航驱动 ---
            //_owner.IsLockOn = false;
            //_owner.agent.isStopped = false;
            _pathUpdateTimer -= Time.deltaTime;
            if (_pathUpdateTimer <= 0f)
            {
                _owner.agent.SetDestination(_owner.GetLockedTargetPosition());
                _pathUpdateTimer = 0.2f;
            }

            // 把导航的理想速度方向转为 Local 坐标，喂给 InputMove
            Vector3 localDir = _owner.transform.InverseTransformDirection(_owner.agent.desiredVelocity.normalized);
            if (localDir.x > 0) localDir.x = 1f;
            else localDir.x = 0;

            if (localDir.z > 0) localDir.z = 1f;
            else localDir.z = 0;
            _owner.InputMove = new Vector2(localDir.x, localDir.z);
            //_owner.InputMove = new Vector2(0.8f, 0.8f);
            _owner.InputRun = true;
            _owner.SetActualRun();

            //_owner.AddMoveLogic();
        }

        public void Exit()
        {
            Debug.Log($"{_owner.gameObject}退出追击状态");

            //_owner.DisableAgent();
            //_owner.agent.isStopped = true;

            _owner.agent.updateRotation = false;

        }
    }
}
