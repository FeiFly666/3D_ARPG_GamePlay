using Assets.StateMachine;
using Assets.StateMachine.CommonState;
using Assets.StateMachine.EnemyState;
using Assets.Combat;
using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Assets.Util;
using Unity.VisualScripting;
using Assets.Model;
using Assets.Script.StateMachine.CommonState;

public class EnemyController : CharacterBase, IInterruptible, IRestrictMove
{
    private StateMachine<EnemyController> stateMachine;

    [SerializeField] private float _detectDistance = 8f;
    [SerializeField] private float _attackDistance = 2f;

    [HideInInspector] public NavMeshAgent agent;

    public Vector2 InputMove = Vector2.zero;

    private float _currentSpeed = 0;

    Vector2 smoothMove;
    Vector2 velocity;

    public bool IsLockOn = false;

    public bool InputRun = false;

    private bool _isActualRunning;
    private float smoothAmount;

    private float _blockTimer;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        //this.LockedTarget = Manager.Character.player.transform;

        agent = this.AddComponent<NavMeshAgent>();

        agent.speed = statusCtrl.runSpeed;

        //agent.isStopped = true;

        rb.useGravity = false;

        Init();
    }
    public override void Init()
    {
        base.Init();

        targetSystem.ChangeMaxDistance( _detectDistance);

        stateMachine = new StateMachine<EnemyController>(this);

        stateMachine.Initialize<EnemyIdleState>();

        this._detectDistance = data.DetectDistance;

    }
    protected override void OnEnable()
    {
        base.OnEnable();

        if(stateMachine != null)
            stateMachine.Initialize<EnemyIdleState>();
    }

    // Update is called once per frame
    void Update()
    {
        _blockTimer -= Time.deltaTime;

        if (this.LockedTargetTransform != null)
        {
            if (AllowLockRotation && IsLockOn)
            {
                RotateToLockTarget();
            }
        }


        stateMachine.Update();
        UpdateAnimator();

        if (agent.isActiveAndEnabled && !agent.updatePosition)
        {
            agent.nextPosition = transform.position;
        }
    }
    public void StartMove()
    {
        agent.updatePosition = true;
        Debug.Log($"{this.gameObject.name} 恢复移动");
    }
    public void StopMove()
    {
        agent.updatePosition = false;
        Debug.Log($"{this.gameObject.name} 停止移动");
    }
    public void ChangeAttackRange(float newRange)
    {
        this._attackDistance = newRange;
        if(statusCtrl.IsPowerUp)
        {
            AttackRangeAdd();
        }
    }
    public void EnableAgent()
    {
        agent.updatePosition = true;
        //agent.updateRotation = true;

        agent.Warp(transform.position);

        agent.isStopped = false;
        agent.ResetPath();
        //agent.enabled = true;
    }

    public void DisableAgent()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;

        agent.isStopped = true;

        agent.ResetPath();
        agent.velocity = Vector3.zero;

        agent.nextPosition = transform.position;
        //agent.enabled = false;
    }
    public float DisToTarget()
    {
        return LockedTargetTransform != null ? (this.transform.position - LockedTargetTransform.position).sqrMagnitude : float.MaxValue;
    }
    public float GetAttackRange()
    {
        return _attackDistance;
    }
    public bool UseSkillLogic()
    {
        bool res = false; 
        if (weaponCtrl.currentWeaponObj != null)
        {
            if (weaponCtrl.weaponData.weaponCombatData.skill.Type == SkillType.TianXiaWuShuang)
            {
                if (!statusCtrl.IsPowerUp && statusCtrl.currentPower >= statusCtrl.maxPower)
                {
                    statusCtrl.PowerUp();
                    res = true;
                }
            }

            else if (statusCtrl.currentPower >= weaponCtrl.weaponData.weaponCombatData.skill.Cost)
            {
                if (weaponCtrl.weaponData.weaponCombatData.skill.Type == SkillType.Action)
                {
                    stateMachine.ChangeState<CharacterSkillState<EnemyController>>();
                    res = true;
                }
                else//加buff
                {

                }
            }
        }
        return res;
    }
    public void RotateToDirection(Vector3 worldDir)
    {
        if (worldDir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(worldDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * 10f
        );
    }
    public void RotateToLockTarget()
    {
        Vector3 dirToEnemy = (LockedTargetTransform.position - this.transform.position).normalized;

        dirToEnemy.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dirToEnemy);

        float speed = 12f;

        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, speed * Time.deltaTime);

    }
    public void SetActualRun()
    {
        _isActualRunning = InputRun && InputMove.sqrMagnitude > 0.01f && !statusCtrl.isExhausted;
    }
    public void AddMoveLogic()
    {
        SetActualRun();

        float originSpeed = _isActualRunning ? statusCtrl.runSpeed : statusCtrl.walkSpeed;

        float directionWeight = Mathf.Lerp(1.0f, 0.7f, Mathf.Clamp01(-InputMove.y));

        float targetSpeed = originSpeed * Mathf.Clamp01(InputMove.magnitude) * directionWeight;

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, 10f * Time.deltaTime);

        if (targetSpeed < 0.01f && _currentSpeed < 0.01f)
        {
            _currentSpeed = 0;
            return;
        }

        Vector3 dir = Vector3.zero;

        if (_isActualRunning)
        {
            float staminaCost = 10f * Time.deltaTime;

            statusCtrl.UseStamina(staminaCost, true);
        }

        if (!IsLockOn)
        {
            // 将模拟摇杆的 X 映射到 X，Y 映射到 Z
            dir = new Vector3(InputMove.x, 0, InputMove.y).normalized;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            //transform.position += transform.forward * dir.magnitude * _currentSpeed * Time.deltaTime;

        }
        else if (LockedTargetTransform != null)
        {
            Vector3 dirToEnemy = (LockedTargetTransform.position - this.transform.position).normalized;

            dirToEnemy.y = 0;

            //RotateToLockTarget(dirToEnemy);

            Vector3 sideDir = Vector3.Cross(Vector3.up, dirToEnemy);

            dir = dirToEnemy * InputMove.y + sideDir * InputMove.x;

            //this.transform.position += dir * _currentSpeed * Time.deltaTime;

        }

        Vector3 movement = dir * _currentSpeed * Time.deltaTime;
        agent.Move(movement);

        transform.position = agent.nextPosition;

        /*        Vector3 pos = transform.position;
                pos.y = agent.nextPosition.y;
                transform.position = pos;*/
    }
    void UpdateAnimator()
    {

        smoothMove = Vector2.SmoothDamp(smoothMove, InputMove, ref velocity, 0.05f);

        float amount = InputMove.magnitude;

        smoothAmount = Mathf.Lerp(smoothAmount, amount, 0.1f);

        anim.SetFloat("AttackSpeed", statusCtrl.attackSpeed);

        anim.SetBool("IsRunning", _isActualRunning);
        anim.SetBool("IsLockOn", IsLockOn);

        if (IsLockOn)
        {
            anim.SetFloat("MoveX", smoothMove.x, 0.05f, Time.deltaTime);
            anim.SetFloat("MoveY", smoothMove.y, 0.05f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("MoveAmount", smoothAmount);
        }
    }
    public void AttackRangeAdd()
    {
        this._attackDistance *= 2;
    }
    public void AttackRangeNormal()
    {
        this._attackDistance /= 2;
    }
    public bool IsDetectTarget()
    {
        if(LockedTargetTransform == null) return false;

        return CombatCalculate.IsInRange(DisToTarget(), this._detectDistance);
    }
    public bool CanAttackPlayer()
    {
        if (LockedTargetTransform == null) return false;

        return CombatCalculate.IsInRange(DisToTarget(), this._attackDistance);
    }
    public void TriggerHit()
    {
        stateMachine.ChangeState<CharacterHitState<EnemyController>>();
    }
    public void TriggerStagger()
    {
        stateMachine.ChangeState<CharacterStaggerState<EnemyController>>();
    }
    public bool TryCancelHit()
    {
        bool res = false;

        float t = Random.Range(0, 100f);
        if(t>60f && statusCtrl.FindEnemy)
        {
            res = true;
            if(t<75f)
            {
                stateMachine.ChangeState<EnemyCombatState>();
            }
            else if(t < 90f)
            {
                stateMachine.ChangeState<EnemyAttackState>();
            }
            else
            {
                float blockNum = Random.Range(0.5f, 1.5f);
                SetAIBlock(blockNum);
            }
        }

        return res;
    }
    public void TriggerBlockHit()
    {
        this.anim.Play("BlockHit");
    }
    public override void RevertToDefaultState()
    {
        if(statusCtrl.FindEnemy)
            stateMachine.ChangeState<EnemyCombatState>();
        else
            stateMachine.ChangeState<EnemyIdleState>();
    }

    public void SetAIBlock(float duration)
    {
        _blockTimer = duration;
        stateMachine.ChangeState<CharacterBlockState<EnemyController>>();
    }

    public override bool IsBlockReleased()
    {
        return _blockTimer <= 0;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState<CharacterDeathState<EnemyController>>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // --- 1. 绘制检测范围 (实线黄圈) ---
        Gizmos.color = Color.yellow;
        DrawWireDisk(transform.position, _detectDistance);

        // --- 2. 绘制攻击范围 (实线红圈) ---
        // 如果玩家已经在攻击范围内，圈圈颜色变深
        Gizmos.color = CanAttackPlayer() ? Color.magenta : Color.red;
        DrawWireDisk(transform.position, _attackDistance);

        // --- 3. 目标指示线 ---
        if (LockedTargetTransform != null)
        {
            // 画一条线连向目标
            Gizmos.color = IsDetectTarget() ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position + Vector3.up, LockedTargetTransform.position + Vector3.up);

            // 在目标头顶画个小方块
            Gizmos.DrawCube(LockedTargetTransform.position + Vector3.up * 2f, new Vector3(0.2f, 0.2f, 0.2f));
        }

        // --- 4. 视口前方指示 (可选) ---
        Gizmos.color = new Color(1, 1, 1, 0.2f);
        Vector3 forward = transform.forward * _detectDistance;
        Gizmos.DrawRay(transform.position + Vector3.up, forward);
    }

    // 辅助工具：在地面画一个平面的圆圈 (比球体更适合 ARPG)
    private void DrawWireDisk(Vector3 center, float radius)
    {
        float angle = 0f;
        Vector3 lastPoint = Vector3.zero;

        // 分段画线，每 15 度一段
        for (int i = 0; i <= 24; i++)
        {
            float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = center.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 nextPoint = new Vector3(x, center.y + 0.05f, z); // 稍微抬高一点防止被地面挡住

            if (i > 0)
            {
                Gizmos.DrawLine(lastPoint, nextPoint);
            }

            lastPoint = nextPoint;
            angle += 15f;
        }
    }
#endif
}