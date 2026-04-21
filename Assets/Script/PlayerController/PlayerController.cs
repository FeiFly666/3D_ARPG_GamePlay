using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Assets.StateMachine;
using Assets.StateMachine.PlayerState;
using Assets.StateMachine.CommonState;
using Assets.Model;
using Assets.Combat;
using static UnityEngine.UI.GridLayoutGroup;
using Assets.Script.StateMachine.CommonState;

public class PlayerController : CharacterBase, IInterruptible, ICounterable
{
    public Vector2 InputMove;

    Vector2 smoothMove;
    Vector2 velocity;

    float smoothAmount;
    float currentSpeed;

    public bool InputRun;
    private bool _isActualRunning;

    public bool InputBlocking;

    public bool IsLockOn;


    public bool InputChangeWeapon;
    private bool weaponChanged = false;

    public bool InputChangeItem;
    private bool itemChanged = false;

    public bool InputAttack;

    public bool InputUseItem;

    public bool InputPowerUp;

    private bool IsSwitching;
    private float switchTimer;

    private StateMachine<PlayerController> stateMachine;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        stateMachine = new StateMachine<PlayerController>(this);
        stateMachine.Initialize<PlayerIdleState>();
        Init();
    }
    public void SetInput(Vector2 move, bool run ,bool weaponChange, bool attack, bool blocking, bool powerUP, bool itemChange, bool useItem)
    {
        InputMove = move;
        InputRun = run;
        InputChangeWeapon = weaponChange;
        InputAttack = attack;
        InputBlocking = blocking;
        InputPowerUp = powerUP;
        InputChangeItem = itemChange;
        InputUseItem = useItem;

    }
    private void Update()
    {
        //AddMoveLogic();
        UpdateTimer();

        if(AllowLockRotation && IsLockOn)
        {
            RotateToLockTarget();
        }

        UpdateAnimator();
        stateMachine.Update();
        ItemChangeLogic();
    }
    private void UpdateTimer()
    {
        if(Time.time - switchTimer > 0.22f && IsSwitching)
        {
            SwitchStop();
        }
    }
    public void StartSwitching()
    {
        IsSwitching = true;
        switchTimer = Time.time;
    }
    private void SwitchStop()
    {
        IsSwitching = false;
    }

    public void UseSkillLogic()
    {
        if (InputPowerUp && weaponCtrl.currentWeaponObj != null)
        {
            if(weaponCtrl.weaponData.weaponCombatData.skill.Type == SkillType.TianXiaWuShuang)
            {
                if (!statusCtrl.IsPowerUp && statusCtrl.currentPower >= statusCtrl.maxPower)
                {
                    statusCtrl.PowerUp();
                }
                return;
            }

            if (statusCtrl.currentPower >= weaponCtrl.weaponData.weaponCombatData.skill.Cost)
            {
                if (weaponCtrl.weaponData.weaponCombatData.skill.Type == SkillType.Action)
                {
                    stateMachine.ChangeState<CharacterSkillState<PlayerController>>();
                }
                else//¼Óbuff
                {

                }
            }
        }
        
    }
    public void WeaponChangeLogic()
    {
        if(InputChangeWeapon && !weaponChanged)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;

            WeaponData newData = weapons[currentWeaponIndex];

            weaponCtrl.ChangeWeapon(newData);
            weaponChanged = true;
        }
        else if(!InputChangeWeapon)
        {
            weaponChanged = false;
        }
    }

    public void ItemChangeLogic()
    {
        if (InputChangeItem && !itemChanged)
        {
            currentItemIndex = (currentItemIndex + 1) % items.Count;

            if(items.Count > 0)
            {
                ItemData newData = items[currentItemIndex];

                ItemCtrl.ChangeItem(newData);
            }
            itemChanged = true;
        }
        else if (!InputChangeItem)
        {
            itemChanged = false;
        }
    }
    public void UseItemLogic()
    {
        if(InputUseItem)
        {
            ItemCtrl.UseItem();
        }
    }

/*    public void RotateToLockTarget(Vector3 dirToEnemy)
    {
        Quaternion lookRot = Quaternion.LookRotation(dirToEnemy);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 12f * Time.deltaTime);
    }*/

    public void RotateToLockTarget()
    {
        if (LockedTargetTransform == null) return;

        Vector3 dirToEnemy = (LockedTargetTransform.position - this.transform.position).normalized;

        dirToEnemy.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dirToEnemy);

        float speed = IsSwitching ? 6f : 12f;

        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, speed * Time.deltaTime);

    }

    public void AddMoveLogic()
    {
        _isActualRunning = InputRun && InputMove.sqrMagnitude > 0.01f && !statusCtrl.isExhausted;

        float originSpeed = _isActualRunning ? statusCtrl.runSpeed : statusCtrl.walkSpeed;

        bool isBackMove = IsLockOn && InputMove.y < 0;

        //Vector2 inputWeight = new Vector2(Mathf.Abs(InputMove.x), Mathf.Abs(InputMove.y));

        float directionWeight = Mathf.Lerp(1.0f, 0.7f, Mathf.Clamp01(-InputMove.y));

        float targetSpeed = originSpeed * Mathf.Clamp01(InputMove.magnitude) * directionWeight;

        //originSpeed = originSpeed * Mathf.Max(directionWeight.x, directionWeight.y) / 1.0f;

        //originSpeed = originSpeed * (isBackMove ? 0.8f : 1f);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 10f * Time.deltaTime);

        Vector3 dir = Vector3.zero;

        if(_isActualRunning)
        {
            float staminaCost = 10f * Time.deltaTime;

            statusCtrl.UseStamina(staminaCost, true);
        }

        if(!IsLockOn)
        {
            Transform mainCam = Camera.main.transform;

            Vector3 forward = mainCam.forward;
            Vector3 right = mainCam.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            dir = forward * InputMove.y + right * InputMove.x;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            transform.position += transform.forward * dir.magnitude * currentSpeed * Time.deltaTime;
        }
        else if(LockedTargetTransform != null)
        {
            Vector3 dirToEnemy = (LockedTargetTransform.position - this.transform.position).normalized;

            dirToEnemy.y = 0;

            //RotateToLockTarget(dirToEnemy);

            Vector3 sideDir = Vector3.Cross(Vector3.up, dirToEnemy);

            dir = dirToEnemy * InputMove.y + sideDir * InputMove.x;

            this.transform.position += dir * currentSpeed * Time.deltaTime;
        }

        //UpdateAnimator();
    }
    void UpdateAnimator()
    {
        smoothMove = Vector2.SmoothDamp(smoothMove, InputMove, ref velocity , 0.05f);

        float amount = InputMove.magnitude;

        smoothAmount = Mathf.Lerp(smoothAmount, amount, 0.1f);

        anim.SetFloat("AttackSpeed", statusCtrl.attackSpeed);

        anim.SetBool("IsRunning", _isActualRunning);
        anim.SetBool("IsLockOn", IsLockOn);

        if(IsLockOn)
        {
            anim.SetFloat("MoveX", smoothMove.x, 0.05f , Time.deltaTime);
            anim.SetFloat("MoveY", smoothMove.y, 0.05f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("MoveAmount", smoothAmount);
        }
    }
    public void TriggerHit()
    {
        stateMachine.ChangeState<CharacterHitState<PlayerController>>();
    }
    public void TriggerBlockHit()
    {
        this.anim.Play("BlockHit");
    }
    public void TriggerStagger()
    {
        stateMachine.ChangeState<CharacterStaggerState<PlayerController>>();
    }
    public override void RevertToDefaultState()
    {
        stateMachine.ChangeState<PlayerIdleState>();
    }
    public bool TryCancelHit()
    {
        bool res = false;
        if(InputAttack)
        {
            stateMachine.ChangeState<PlayerAttackState>();
            res = true;
        }
        else if(InputMove != Vector2.zero)
        {
            stateMachine.ChangeState<PlayerMoveState>();
            res = true;
        }
        return res;
    }
    public bool TryExecuteCounter()
    {
        if(InputAttack && weaponCtrl.currentWeaponType != WeaponType.None)
        {
            if(statusCtrl.CanUseStamina(weaponCtrl.weaponData.weaponCombatData.combatData[0].comboStaminaCost))
            {
                stateMachine.ChangeState<PlayerAttackState>();
                return true;
            }
        }
        
        return false;
    }

    public override bool IsBlockReleased() => !InputBlocking;

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState<CharacterDeathState<PlayerController>>();
    }

}
