using Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.StateMachine.EnemyState
{
    public class EnemyCombatState : IState<EnemyController>
    {
        private EnemyController _owner;
        private StateMachine<EnemyController> _fsm;


        private float _attackTimer;
        private float _decisionTimer;
        private float _blockTimer;
        private float _skillTimer;
        float index = 1;

        private float _strafeDirection; // 1 为右，-1 为左
        public void Init(EnemyController owner, StateMachine<EnemyController> machine)
        {
            _owner = owner;
            _fsm = machine;
        }
        public void Enter()
        {
            Debug.Log($"{_owner.gameObject}进入作战状态");
            _owner.currentState = CharacterState.Move;

            string nextAnim = _owner.anim.GetBool("IsLockOn") ? "LockOnForwardWalk" : "Walk";
            _owner.anim.CrossFadeInFixedTime(nextAnim, 0.15f);

            _owner.agent.Warp(_owner.transform.position);

            _owner.DisableAgent();

            _owner.IsLockOn = true;

            float randomT = Random.Range(0, 1.0f);

            _strafeDirection = randomT > 0.5f ? 1 : -1;
            index = Random.Range(0.6f, 0.8f);
        }
        public void Update()
        {
            float dis = _owner.DisToTarget();

            if(dis > 36f)
            {
                _fsm.ChangeState<EnemyChaseState>();
                return;
            }
            _decisionTimer -= Time.deltaTime;
            if (_decisionTimer <= 0)
            {
                _strafeDirection = Random.value > 0.5f ? 1 : -1;
                _decisionTimer = Random.Range(1f, 3f);
                index = Random.Range(0.6f, 0.8f);
            }

            _blockTimer -= Time.deltaTime;
            if (_blockTimer <= -10)
            {
                _blockTimer = -10;
            }

            if(_owner.lockedTarget.currentState == CharacterState.Attack && _blockTimer <= 0)
            {
                if (Random.value < 0.6f && dis <= 9f)
                {
                    _owner.SetAIBlock(Random.Range(0.3f, 1.5f));
                    return;
                }
                else
                {
                    _blockTimer = Random.Range(0.5f, 1.5f);
                }
            }

            _skillTimer -= Time.deltaTime;
            if(_owner.statusCtrl.currentPower >= _owner.weaponCtrl.currentWeaponObj.data.weaponCombatData.skill.Cost && _skillTimer < 0)
            {
                if(_owner.UseSkillLogic())
                {
                    _skillTimer = Random.Range(1f, 20f);
                    return;

                }
            }
           
            float xInput = _strafeDirection * index;
            float yInput = 0;

            if (dis > _owner.GetAttackRange()) yInput = 1f;
            else if (dis < 1f) yInput = -1f;

            if(yInput != 0)
            {
                _owner.InputMove = new Vector2(0, yInput);
            }
            else
            {
                _owner.InputMove = new Vector2(xInput, 0);
            }
            _owner.InputRun = false;

            _owner.AddMoveLogic();

            float attackT = Random.Range(1, 100f);
            _attackTimer -= Time.deltaTime;

            if (_owner.CanAttackPlayer() && _attackTimer <= 0 && attackT > 0.5f)
            {
                _fsm.ChangeState<EnemyAttackState>();
                _attackTimer = Random.Range(1f, 5f);
            }
        }

        public void Exit()
        {
            //_owner.IsLockOn = false;
            Debug.Log($"{_owner.gameObject}退出作战状态");
        }
    }
}
