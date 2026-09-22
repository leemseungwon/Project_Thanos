using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Enemy.GolemEnemies.Components;
using LSW._02._Scripts.Entity.Enemy.GolemEnemies.States;
using LSW._03._So.Attack_Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies
{
    public class GolemEnemy : BaseEnemy
    {
        [Header("Attack Probability")] 
        [Range(0f, 1f)] [SerializeField] private float clapAttackChance = 0.3f;

        [Header("Attack Decision")]
        [SerializeField] private float attackDecisionInterval = 1f;
        
        private GolemCombatComponent _golemCombat;
        private GolemAnimation _golemAnimation;

        private float _decisionTimer;

        public GolemCombatComponent GolemCombat => _golemCombat;
        public GolemAnimation GolemAnimation => _golemAnimation;
        
        public const string IdleState = "GolemIdleState";
        public const string ChaseState = "GolemChaseState";
        public const string FrontAttackState = "GolemFrontAttackState";
        public const string ClapState = "GolemClapState";
        public const string TakeDamageState = "GolemTakeDamageState";

        protected override void Awake()
        {
            base.Awake();

            CreateStateMachine();
        }

        private void Start()
        {
            if(Animation != null && Animation is GolemAnimation golemAnimation)
                _golemAnimation = golemAnimation;
            GetCompo(out _golemCombat);
            
            BindAnimationEvents();
            
            StateMachine.Initialize(IdleState);
            Spawn(); // 없앨 예정
        }

        private void CreateStateMachine()
        {
            StateMachine = new EntityStateMachine(
                    new List<StateData>
                    {
                        new StateData { stateName = IdleState, State = new GolemIdleState(this) },
                        new StateData { stateName = ChaseState, State = new GolemChaseState(this) },
                        new StateData { stateName = FrontAttackState, State = new GolemFrontAttackState(this) },
                        new StateData { stateName = ClapState, State = new GolemClapState(this) },
                        new StateData { stateName = TakeDamageState, State = new GolemTakeDamageState(this) }
                    }
                );
        }

        public override void Spawn()
        {
            base.Spawn();

            _decisionTimer = 0f;
        }

        protected override void Update()
        {
            base.Update();

            if (IsDead)
                return;

            if (_decisionTimer > 0f)
            {
                _decisionTimer -= Time.deltaTime;
            }
        }

        public bool TryChooseAttack()
        {
            if (_golemCombat == null || _decisionTimer > 0f)
                return false;

            // 이미 공격 중이거나 피격 중인 경우 새로운 공격을 선택하지 않음
            if (StateMachine.CurrentState?.stateName != ChaseState && 
                StateMachine.CurrentState?.stateName != IdleState)
                return false;

            if (Random.value < clapAttackChance && _golemCombat.CanClapAttack)
            {
                _decisionTimer = attackDecisionInterval;
                ChangeState(ClapState);
                return true;
            }
            
            if (_golemCombat.CanFrontAttack)
            {
                _decisionTimer = attackDecisionInterval;
                ChangeState(FrontAttackState);
                return true;
            }

            return false;
        }

        public override bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower, float stunDuration, 
            BaseEntity attacker, AttackData attackData)
        {
            if (IsDead)
                return false;

            bool isSuccess = base.TakeDamage(finalDamage, knockBackDirection, knockBackPower, stunDuration, attacker, attackData);

            if (isSuccess && !IsDead)
            {
                ChangeState(TakeDamageState);
            }

            return isSuccess;
        }

        public void ChangeState(string stateName)
        {
            if (StateMachine == null)
                return;

            StateMachine.ChangeState(stateName);
        }

        
        #region Animation Events

        private void BindAnimationEvents()
        {
            if (_golemAnimation == null)
                return;
            
            _golemAnimation.OnFrontAttackCast += HandleFrontAttackCast;
            _golemAnimation.OnFrontAttackAnimationEnd += HandleFrontAttackEnd;
            _golemAnimation.OnClapAttackCast += HandleClapCast;
            _golemAnimation.OnClapAttackAnimationEnd += HandleClapEnd;
            _golemAnimation.OnTakeDamageAnimationEnd += HandleTakeDamageEnd;
        }

        private void UnbindAnimationEvents()
        {
            if (_golemAnimation == null)
                return;
            
            _golemAnimation.OnFrontAttackCast -= HandleFrontAttackCast;
            _golemAnimation.OnFrontAttackAnimationEnd -= HandleFrontAttackEnd;
            _golemAnimation.OnClapAttackCast -= HandleClapCast;
            _golemAnimation.OnClapAttackAnimationEnd -= HandleClapEnd;
            _golemAnimation.OnTakeDamageAnimationEnd -= HandleTakeDamageEnd;
        }

        private void HandleFrontAttackCast() => CallStateAction<GolemFrontAttackState>(state => state.AttackCast());
        private void HandleFrontAttackEnd() => CallStateAction<GolemFrontAttackState>(state => state.AnimationEnd());
        private void HandleClapCast() => CallStateAction<GolemClapState>(state => state.AttackCast());
        private void HandleClapEnd() => CallStateAction<GolemClapState>(state => state.AnimationEnd());
        private void HandleTakeDamageEnd() => CallStateAction<GolemTakeDamageState>(state => state.AnimationEnd());

        
        private void CallStateAction<T>(Action<T> action) where T : class
        {
            if (StateMachine.CurrentState?.State is T state)
            {
                action?.Invoke(state);
            }
        }

        #endregion

        protected override void OnDeath()
        {
            Movement.EnableMovement(false);

            base.OnDeath();
        }

        public override void OnDestroy()
        {
            UnbindAnimationEvents();

            base.OnDestroy();
        }

        public override void Despawn()
        {
        }
    }
}
