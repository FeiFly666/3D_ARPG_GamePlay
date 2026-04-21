using Assets.Combat;
using Assets.Model;
using Cinemachine;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour, IDamageable
{
    private CharacterBase _owner;
    [SerializeField] private bool DebugPerfectParray = false;
    [SerializeField] private bool DebugMaxPower = false;

    [Header("血量")]
    public int FullHP;
    public float currentHP;

    [Header("韧性")]
    public float maxPoise = 100f;
    public float currentPoise;
    public float poiseRecoverRate = 15f;
    public float poiseRecoverDelay = 2f;

    private float _lastHitTime;

    public bool isStagger = false;

    public bool CanRecoverPoise = true;


    [Header("体力")]
    public float maxStamina = 100f;
    public float currentStamina;

    public float exhaustedStaminaRecoverDelay = 2f;
    public float normalStaminaRecoverDelay = 0.5f;

    public float staminaRecoverRate = 25f;
    private float _lastUseStaminaTime;

    public bool isExhausted { get; private set; }

    public bool CanRecoverStamina = true;

    [Header("技能相关")]
    public float maxPower = 150f;
    public float currentPower = 0f;

    [Header("天下无双")]

    public float powerDreaseRate = 4f;

    public bool IsPowerUp;

    [Header("属性")]
    public float attack;

    public float attackSpeed = 1.0f;

    public int defense;

    [Header("速度")]
    public float runSpeed = 5f;
    public float walkSpeed = 2f;

    [Header("运行时状态")]
    public bool IsPerfectBlock;
    public bool IsBlocking;
    public bool blockedPerfectly;
    public bool FindEnemy => _owner.lockedTarget != null ;

    //事件
    public System.Action<float,float> OnHealthChanged;
    public System.Action<float,float> OnStaminaChanged;
    public System.Action<float,float> OnPoiseChanged;
    public System.Action<float,float> OnPowerChanged;

    public void Init(CharacterBase character)
    {
        _owner = character;

        this.FullHP = _owner.data.FullHP;
        this.currentHP = this.FullHP;

        this.attack = _owner.data.ATK;
        this.defense = _owner.data.DEF;

        this.maxStamina = _owner.data.MaxStamina;
        this.currentStamina = this.maxStamina;

        this.maxPoise = _owner.data.MaxPoise;
        this.currentPoise = this.maxPoise;

        this.attackSpeed = 1.0f;

        this.walkSpeed = _owner.data.WalkSpeed;

        this.runSpeed = _owner.data.RunSpeed;

        InvokeUIChange();
    }
    public void InvokeUIChange()
    {
        OnHealthChanged?.Invoke(currentHP, FullHP);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        OnPoiseChanged?.Invoke(currentPoise, maxPoise);
    }

    private void Update()
    {
        HandleStaminaRecovery();
        HandlePoiseRecovery();
        HandlePowerDrecrease();

        if(DebugMaxPower)
        {
            currentPower = maxPower;
        }
    }

    public void RecoverHP(float index)
    {
        currentHP = Mathf.Min(FullHP, currentHP + index);

        OnHealthChanged?.Invoke(currentHP, FullHP);
    }

    #region 天下无双相关
    public void PowerUp()
    {
        IsPowerUp = true;
        attackSpeed = 1.5f;
        if(_owner is EnemyController owner)
        {
            owner.AttackRangeAdd();
        }
    }
    public void PowerUpEnd()
    {
        IsPowerUp = false;
        attackSpeed = 1.0f;
        if (_owner is EnemyController owner)
        {
            owner.AttackRangeNormal();
        }
        OnPowerChanged?.Invoke(currentPower, maxPower);
    }
    public void IncreasePower(float amount)
    {
        currentPower += amount;

        if(currentPower >= maxPower) currentPower = maxPower;

        OnPowerChanged?.Invoke(currentPower, maxPower);
    }
    public bool DecreasePower(float amount)
    {
        if (currentPower < amount) return false;

        currentPower -= amount;

        OnPowerChanged?.Invoke(currentPower, maxPower);

        return true;
    }
    private void HandlePowerDrecrease()
    {
        if (!IsPowerUp) return;

        float decrease = powerDreaseRate * Time.deltaTime;

        currentPower -= decrease;
        if( currentPower <= 0 )
        {
            currentPower = 0;

            PowerUpEnd();
        }
        OnPowerChanged?.Invoke(currentPower, maxPower);
    }
    #endregion

    #region 体力相关
    private void HandleStaminaRecovery()
    {
        if(!CanRecoverStamina) return;

        float delay = isExhausted ? exhaustedStaminaRecoverDelay : normalStaminaRecoverDelay;
        if(Time.time - _lastUseStaminaTime > delay)
        {
            if(currentStamina < maxStamina)
            {
                float recover = staminaRecoverRate * Time.deltaTime;

                if (IsPowerUp) recover *= 1.2f;

                currentStamina += recover;
                currentStamina = Mathf.Min(currentStamina,maxStamina);
            }

            if(currentStamina > maxStamina * 0.3f && isExhausted)
            {
                isExhausted = false;
                Debug.Log($"{_owner.gameObject.name}不再力竭了");
            }

            OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        }
    }
    public bool CanUseStamina(float amount)
    {
        if (IsPowerUp)
        {
            amount /= 2;
        }
        return isExhausted ? currentStamina >= amount : currentStamina > 0;
    }
    public bool UseStamina(float amount, bool force = false)
    {
        if(IsPowerUp)
        {
            amount /= 2;
        }

        if(isExhausted)
        {
            if(force)
                return false;

            else if (currentStamina >= amount)
            {
                currentStamina -= amount;
                _lastUseStaminaTime = Time.time;

                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                return true;
            }
            return false;
        }
        else
        {
            currentStamina -= amount;
            _lastUseStaminaTime = Time.time;

            if (currentStamina <= 0)
            {
                currentStamina = 0;

                isExhausted = true;

                Debug.Log($"{_owner.gameObject.name}力竭了");
            }

            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        return true;
    }
    public void ReduceStamina(float amount)
    {
        if (IsPowerUp)
        {
            amount /= 2;
        }

        currentStamina -= amount;
        _lastUseStaminaTime = Time.time;

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        UseStamina(0);
    }
    #endregion

    #region 韧性相关

    public void RecoverPoise(float amount)
    {
        this.currentPoise += amount;

        if(currentPoise > maxPoise) { currentPoise = maxPoise; }

        OnPoiseChanged?.Invoke(currentPoise, maxPoise);
    }
    public void ReducePoise(float amount)
    {
        this.currentPoise -= amount;

        _lastHitTime = Time.time;

        if (currentPoise > 0)
        {
            //isStagger = false;
            if(_owner is IInterruptible owner)
            {
                if(!isStagger || isStagger && currentPoise >= maxPoise * 0.15f)
                    owner.TriggerHit();
            }
        }
        else
        {
            //isStagger = true;
            currentPoise = Mathf.Max(0, currentPoise);
            if (_owner is IInterruptible owner)
            {
                owner.TriggerStagger();
            }
        }

        OnPoiseChanged?.Invoke(currentPoise, maxPoise);
    }

    private void HandlePoiseRecovery()
    {
        if (!CanRecoverPoise) return;

        if (Time.time - _lastHitTime > poiseRecoverDelay || isStagger)
        {
            /*if(currentPoise > maxPoise * 0.2f && isStagger)
            {
                isStagger = false;
            }*/
            if (currentPoise < maxPoise)
            {
                float mult = currentPoise < maxPoise * 0.1f ? 3 : 1;

                currentPoise += poiseRecoverRate * mult * Time.deltaTime;
                currentPoise = Mathf.Min(currentPoise, maxPoise);
            }
            OnPoiseChanged?.Invoke(currentPoise, maxPoise);
        }
    }

    #endregion
    public void TakeDamage(float damage, int poiseDamage, CharacterBase attacker)
    {
        Vector3 dirToAttacker = (attacker.transform.position - transform.position).normalized;
        dirToAttacker.y = 0;
        
        bool isFrontal = Vector3.Dot(transform.forward, dirToAttacker) > 0.5f;

        bool blockedSuccessfully = false;
        
        if(!FindEnemy)
        {
            //FindEnemy = true;
            _owner.SetLockedTarget(attacker);
            if(_owner.characterType == CharacterType.Boss)
            {
                Manager.Event.Execute(EventManager.Event_Type.Boss_Find_Player, _owner as EnemyController);
            }
        }

        if(IsBlocking && isFrontal)
        {
            if(this.IsPerfectBlock || this.DebugPerfectParray)
            {
                //isPerfect = true;
                blockedSuccessfully = true;
                blockedPerfectly = true;

                damage = 0;
                poiseDamage = 0;

                IncreasePower(maxPower * 0.25f);

                StartCoroutine(PerfectBlockEffect());
                Debug.Log("<color=cyan>【完美格挡】！</color>");

                if(attacker!= null)
                {
                    if (attacker.characterType == CharacterType.Enemy || attacker.characterType == CharacterType.Player)
                    {
                        attacker.statusCtrl.ReducePoise(attacker.statusCtrl.maxPoise * 0.7f);
                    }
                    else if (attacker.characterType == CharacterType.Boss)
                    {
                        attacker.statusCtrl.ReducePoise(attacker.statusCtrl.maxPoise * 0.25f);
                    }
                }

                return;

            }
            else
            {
                float staminaCost = poiseDamage * 0.8f;
                float reducePoiseDamage = poiseDamage * 0.4f;

                ReduceStamina(staminaCost);
                if (currentStamina <= 0)
                {
                    Debug.Log("<color=red>【格挡被破防】！体力不足</color>");
                    currentStamina = 0;
                    currentPoise = 0;
                    blockedSuccessfully = false;
                }
                else if (currentPoise <= reducePoiseDamage)
                {
                    Debug.Log("<color=orange>【格挡被重击击穿】！韧性不足</color>");
                    currentPoise = 0; // 后面会自动触发 Stagger
                    blockedSuccessfully = false;
                }
                else
                {
                    damage = Mathf.RoundToInt(damage * 0.2f);
                    poiseDamage = Mathf.RoundToInt(reducePoiseDamage);

                    blockedSuccessfully = true;
                    Debug.Log($"普通格挡：消耗体力 {staminaCost} 消耗韧性 {poiseDamage}");
                }
            }
        }

        this.currentHP -= damage;
        if (!isStagger)
        {
            this.currentPoise -= poiseDamage;
        }

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        OnPoiseChanged?.Invoke(currentPoise, maxPoise);
        OnHealthChanged?.Invoke(currentHP, FullHP);

        _lastHitTime = Time.time;

        if (this.currentHP <= 0)
        {
            _owner?.Die();
            return;
        }

        IInterruptible interruptible = _owner as IInterruptible;

        if (currentPoise > 0)
        {
            //isStagger = false;

            //if (IsPowerUp) return;

            if (blockedSuccessfully)
            {
                interruptible?.TriggerBlockHit();
            }
            else
            {
                if(!isStagger || isStagger && currentPoise >= maxPoise * 0.15f)
                    interruptible?.TriggerHit();
            }
        }
        else
        {
            /*if(isStagger == true)
            {
                if(currentPoise < maxPoise * 0.15f)
                    interruptible?.TriggerHit();
                return;
            }*/
            currentPoise = 0;
            interruptible?.TriggerStagger();
        }
        
    }
    IEnumerator PerfectBlockEffect()
    {
        VFXHelper.Play(_owner.transform, _owner.weaponCtrl.currentWeaponObj.transform, _owner.weaponCtrl.weaponData.weaponCombatData.ParrayVFX);

        yield return new WaitForSecondsRealtime(0.16f);

        Time.timeScale = 0.02f;

        yield return new WaitForSecondsRealtime(0.4f);
        Time.timeScale = 1f;

        blockedPerfectly = false;
    }


}
