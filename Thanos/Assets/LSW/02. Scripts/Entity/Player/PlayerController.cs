using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.Entity.Player.States;
using LSW._03._So.Entity_Stats.Player;
using LSW._03._So.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSW._02._Scripts.Entity.Player
{
    public class PlayerController : BaseEntity
    {
        [Header("Run Resources")]
        [SerializeField, Min(0)] private int startingCurrency = 30;
        public int Currency { get; private set; }
        public event Action<int> OnCurrencyChanged;
        public event Action<int, int, float> OnStatChanged;
        public event Action<float, float> OnDashCooldownChanged;
        
        [Header("Input")]
        [field: SerializeField] public InputData Input { get; private set; }
        
        [Header("Camera")]
        [field:SerializeField] public Camera PlayerCamera { get; private set; }
        
        public PlayerMovementComponent Movement { get; private set; }
        public PlayerCombatComponent Combat { get; private set; }
        public PlayerStoneComponent Stone { get; private set; }
        public bool CanDash => _dashCooldownTimer <= 0f;
        public float DashCooldownRemaining => Mathf.Max(0f, _dashCooldownTimer);
        public float DashCooldownDuration => _playerStatData.dashCooldown;
        public float DashCooldownProgress => DashCooldownDuration <= 0f
            ? 1f
            : 1f - DashCooldownRemaining / DashCooldownDuration;
        
        public Vector2 LookDirection { get; private set; }

        private PlayerAnimation playerAnimation;
        private PlayerStatData _playerStatData;
        private float _dashCooldownTimer;
        
        public const string IdleState = "PlayerIdleState";
        public const string MoveState = "PlayerMoveState";
        public const string AttackState = "PlayerAttackState";
        public const string DashState = "PlayerDashState";
        
        protected override void Awake()
        {
            base.Awake();
            Currency = Mathf.Max(0, startingCurrency);
            _playerStatData = StatData as PlayerStatData;
            
            playerAnimation = Animation as PlayerAnimation;
            
            if (GetCompo(out PlayerMovementComponent movement))
                Movement = movement;

            if (GetCompo(out PlayerCombatComponent combat))
                Combat = combat;
            
            if (GetCompo(out PlayerStoneComponent stone))
                Stone = stone;

            InitStates();

            if (Input != null)
            {
                Input.OnSwitchNextPressed += HandleStoneSwitch;
                Input.OnAttackPressed += HandleAttack;
                Input.OnDashPressed += HandleDash;
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
                }
            );
        }
        
        private void Start()
        {
            if (Input != null)
                Input.InitializeInput();

            StateMachine.Initialize(IdleState);
        }

        protected override void Update()
        {
            base.Update();
            UpdateDashCooldown();

            bool isDashing = StateMachine != null &&
                             StateMachine.CurrentState.HasValue &&
                             StateMachine.CurrentState.Value.stateName == DashState;
            if (!Combat.IsAttacking && !isDashing)
                UpdateLookDirection();
            
            OnStatChanged?.Invoke(CurrentHp, MaxHp, DashCooldownProgress);
        }

        public void EnableInput(bool enable)
        {
            if(Input != null)
                Input.EnableInput(enable);
            if (Movement != null)
                Movement.EnableMovement(enable);
        }

        public bool TrySpendCurrency(int amount)
        {
            if (amount < 0 || Currency < amount)
                return false;

            Currency -= amount;
            OnCurrencyChanged?.Invoke(Currency);
            return true;
        }

        public void AddCurrency(int amount)
        {
            if (amount <= 0)
                return;

            Currency += amount;
            OnCurrencyChanged?.Invoke(Currency);
        }

        public void UpdateLookDirection()
        {
            if (PlayerCamera == null || Input == null)
                return;

            Vector3 mouseWorldPosition = PlayerCamera.ScreenToWorldPoint(new Vector3(
                Input.MousePosition.x, 
                Input.MousePosition.y,
                Mathf.Abs(PlayerCamera.transform.position.z - transform.position.z)));
            
            SetLookDirection(mouseWorldPosition - transform.position);
        }

        public void SetLookDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            LookDirection = direction.normalized;
            if (playerAnimation != null)
            {
                playerAnimation.SetParam(playerAnimation.LookDirectionXAnimationData, LookDirection.x);
                playerAnimation.SetParam(playerAnimation.LookDirectionYAnimationData, LookDirection.y);
            }
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
            if (StateMachine == null || !Movement.IsEnabledMovement || !CanDash)
                return;

            _dashCooldownTimer = Mathf.Max(0f, DashCooldownDuration);
            OnDashCooldownChanged?.Invoke(DashCooldownRemaining, DashCooldownDuration);
            StateMachine.ChangeState(DashState);
        }

        private void UpdateDashCooldown()
        {
            if (_dashCooldownTimer <= 0f)
                return;

            _dashCooldownTimer = Mathf.Max(0f, _dashCooldownTimer - Time.deltaTime);
            OnDashCooldownChanged?.Invoke(DashCooldownRemaining, DashCooldownDuration);
        }

        private void HandleAttack()
        {
            if (StateMachine == null || Combat == null)
                return;
            
            if (!Combat.CanAttack)
                return; 
            
            StateMachine.ChangeState(AttackState);
        }

        public bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower, float stunDuration, 
            BaseEntity attacker)
        {
            return base.TakeDamage(finalDamage, knockBackDirection, knockBackPower, stunDuration, attacker);
        }

        public override void OnDestroy()
        {
            if (Input != null)
            {
                Input.OnSwitchNextPressed -= HandleStoneSwitch;
                Input.OnAttackPressed -= HandleAttack;
                Input.OnDashPressed -= HandleDash;
            }

            base.OnDestroy();
        }
    }
}
