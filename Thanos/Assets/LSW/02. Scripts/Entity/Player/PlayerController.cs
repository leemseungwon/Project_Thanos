using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.Entity.Player.States;
using LSW._03._So.Inputs;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player
{
    public class PlayerController : BaseEntity
    {
        [Header("Input")]
        [field: SerializeField] public InputData Input { get; private set; }
        
        [Header("Camera")]
        [field:SerializeField] public Camera PlayerCamera { get; private set; }
        
        public PlayerMovementComponent Movement { get; private set; }
        public PlayerCombatComponent Combat { get; private set; }
        public PlayerStoneComponent Stone { get; private set; }
        public PlayerParryComponent Parry { get; private set; }
        
        public Vector2 LookDirection { get; private set; }
        
        private Vector3 _originalPosition;
        private Vector3 _originalScale;
        private bool _isParrying;
        
        public const string IdleState = "PlayerIdleState";
        public const string MoveState = "PlayerMoveState";
        public const string AttackState = "PlayerAttackState";
        public const string DashState = "PlayerDashState";
        public const string ParryState = "PlayerParryState";
        
        protected override void Awake()
        {
            base.Awake();

            if (Animation != null)
            {
                _originalPosition = Animation.transform.localPosition;
                _originalScale = Animation.transform.localScale;
            }
            
            if (GetCompo(out PlayerMovementComponent movement))
                Movement = movement;

            if (GetCompo(out PlayerCombatComponent combat))
                Combat = combat;
            
            if (GetCompo(out PlayerStoneComponent stone))
                Stone = stone;

            if (GetCompo(out PlayerParryComponent parry))
                Parry = parry;

            InitStates();

            if (Input != null)
            {
                Input.OnSwitchNextPressed += HandleStoneSwitch;
                Input.OnAttackPressed += HandleAttack;
                Input.OnDashPressed += HandleDash;
                Input.OnParryPressed += HandleParry;
            }
        }

        private void InitStates()
        {
            StateMachine = new EntityStateMachine
            (
                new List<StateData>{
                    new StateData
                    {
                        stateName = IdleState,
                        State = new PlayerIdleState(this)
                    },
                    new StateData
                    {
                        stateName = MoveState,
                        State = new PlayerMoveState(this)
                    },
                    new StateData
                    {
                        stateName = AttackState,
                        State = new PlayerAttackState(this)
                    },
                    new StateData
                    {
                        stateName = DashState,
                        State = new PlayerDashState(this)
                    },
                    new StateData
                    {
                        stateName = ParryState,
                        State = new PlayerParryState(this)
                    }
                }
            );
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        protected override void Update()
        {
            base.Update();
            LookAtMouse();
        }

        private void LookAtMouse()
        {
            Vector3 mouseWorldPosition =
                PlayerCamera.ScreenToWorldPoint(new Vector3(Input.MousePosition.x, Input.MousePosition.y,
                        Mathf.Abs(PlayerCamera.transform.position.z - transform.position.z)));

            bool isLeft = mouseWorldPosition.x < transform.position.x;
            Vector3 position = _originalPosition;
            position.x = isLeft ? -_originalPosition.x : _originalPosition.x;

            Animation.transform.localPosition = position;

            Animation.transform.localScale = new Vector3(isLeft ? -Mathf.Abs(_originalScale.x) 
                    : Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);

            LookDirection = isLeft ? Vector2.left : Vector2.right;
        }
        
        private void HandleParry()
        {
            if (StateMachine == null || _isParrying || Combat.IsAttacking)
                return;

            if (Parry == null || !Parry.CanParry)
                return;

            StateMachine.ChangeState(ParryState, () => _isParrying = false);
            _isParrying = true;
        }

        private void HandleStoneSwitch(bool isNext)
        {
            if (isNext)
            {
                Stone.SwitchNext();
            }
            else
            {
                Stone.SwitchPrevious();
            }
        }

        private void HandleDash()
        {
            if(StateMachine == null || !Movement.IsEnabledMovement || _isParrying || Combat.IsAttacking)
                return;
            StateMachine.ChangeState(DashState);
        }

        private void HandleAttack()
        {
            if (StateMachine == null || _isParrying || Combat == null)
                return;
            
            if (Combat.IsAttacking)
            {
                Combat.TryAttack();
                return;
            }
            
            if (Combat.IsWaitingForNextAttack)
            {
                if (Combat.TryAttack())
                {
                    StateMachine.ChangeState(AttackState);
                }

                return;
            }

            if (!Combat.CanAttack)
                return;
            
            StateMachine.ChangeState(AttackState);
        }
        
        public override bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower = 0f, float stunDuration = 0f, BaseEntity attacker = null)
        {
            if (Parry != null && Parry.TryParry(attacker, finalDamage, knockBackPower, stunDuration))
            {
                return true;
            }

            return base.TakeDamage(finalDamage, knockBackDirection, knockBackPower, stunDuration, attacker);
        }

        private void OnDestroy()
        {
            if (Input != null)
            {
                Input.OnSwitchNextPressed -= HandleStoneSwitch;
                Input.OnAttackPressed -= HandleAttack;
                Input.OnDashPressed -= HandleDash;
                Input.OnParryPressed -= HandleParry;
            }
        }
    }
}