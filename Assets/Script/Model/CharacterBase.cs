using Assets.Anim;
using Assets.Combat;
using Assets.Model;
using Assets.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Model
{
    public enum CharacterType
    {
        Player,
        Boss,
        NPC,
        Enemy
    }
    public enum CharacterSex
    { 
        Female,
        Male
    }
    public enum CharacterState
    {
        Idle,
        Move,
        Attack,
        Block,
        Hit,
        Skill,
        Dead
    }
    public enum WeaponType
    {
        None,
        OH,
        TH
    }
    public abstract class CharacterBase : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] private AnimEventDispatcher _animDispatcher;
        [SerializeField] public CharacterData data;

        public CharacterType characterType;

        public CharacterSex characterSex;

        public CharacterState currentState;

        public LayerMask enemyLayer;
        public LayerMask obstacleLayer;

        public WeaponController weaponCtrl;
        public StatusController statusCtrl;

        public TargetSystem targetSystem;

        public CharacterBase lockedTarget;

        protected Transform LockedTargetTransform;

        public bool AllowLockRotation = true;

        protected Rigidbody rb;

        public List<WeaponData> weapons = new List<WeaponData>();
        [SerializeField] protected int currentWeaponIndex = 0;

        protected virtual void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            _animDispatcher = GetComponentInChildren<AnimEventDispatcher>();

            if(_animDispatcher != null)
            {
                _animDispatcher.onRightAttackStart += AnimationEvent_Right_StartHit;
                _animDispatcher.onRightAttackEnd += AnimationEvent_Right_StopHit;
                _animDispatcher.onLeftAttackStart += AnimationEvent_Left_StartHit;
                _animDispatcher.onLeftAttackEnd += AnimationEvent_Left_StopHit;
            }

            rb = GetComponent<Rigidbody>();

            weaponCtrl = GetComponent<WeaponController>();
            if(weaponCtrl == null) weaponCtrl = this.AddComponent<WeaponController>();

            statusCtrl = GetComponent<StatusController>();
            if (statusCtrl == null) statusCtrl = this.AddComponent<StatusController>();

            targetSystem = GetComponent<TargetSystem>();
            if(targetSystem == null) { targetSystem = this.AddComponent<TargetSystem>(); }

            //Init();
        }
        protected virtual void Start()
        {
            Manager.Character.RegisterCharacter(this);
        }
        protected virtual void OnEnable()
        {
            statusCtrl.Init(this);

            //weaponCtrl.Init(this, weapons[currentWeaponIndex]);
        }
        public virtual void Init()
        {
            statusCtrl.Init(this);

            weaponCtrl.Init(this, weapons[currentWeaponIndex]);

            targetSystem.Init(enemyLayer, obstacleLayer, this);

        }
        private void InitEnemyLayer()
        {

        }
        public virtual void SetLockedTarget(CharacterBase lockedTarget)
        {
            this.lockedTarget = lockedTarget;
            LockedTargetTransform = lockedTarget.transform;
        }
        public virtual void UnLockTarget()
        {
            lockedTarget = null;
            LockedTargetTransform = null;
        }
        public Vector3 GetLockedTargetPosition()
        {
            return LockedTargetTransform.position;
        }
        public void ChangeUpBodyWeight(float leftWeight, float rightWeight)
        {
            anim.SetLayerWeight(1, leftWeight);
            anim.SetLayerWeight(2, rightWeight);
        }

        public void AnimationEvent_Right_StartHit()
        {
            if (weaponCtrl != null && weaponCtrl.currentWeaponObj != null)
            {
                weaponCtrl.currentWeaponObj.StartDetect();
            }
        }
        public void AnimationEvent_Left_StartHit()
        {
            if (weaponCtrl != null && weaponCtrl.currentWeaponObj2 != null)
            {
                weaponCtrl.currentWeaponObj2.StartDetect();
            }
        }
        public void AnimationEvent_Right_StopHit()
        {
            if (weaponCtrl != null && weaponCtrl.currentWeaponObj != null)
            {
                weaponCtrl.currentWeaponObj.StopDetect();
            }
        }
        public void AnimationEvent_Left_StopHit()
        {
            if (weaponCtrl != null && weaponCtrl.currentWeaponObj2 != null)
            {
                weaponCtrl.currentWeaponObj2.StopDetect();
            }
        }
/*        public virtual void TriggerHit()
        {

        }*/
/*        public virtual void TriggerBlockHit()
        {

        }*/
        /*public virtual void TriggerStagger()
        {

        }*/
        public abstract void RevertToDefaultState();
        //public virtual bool TryCancelHit() => false;

        public abstract bool IsBlockReleased();
        public virtual void Die()
        {
            Debug.Log($"{gameObject.name}À¿Õˆ");
            
        }
    }
}
