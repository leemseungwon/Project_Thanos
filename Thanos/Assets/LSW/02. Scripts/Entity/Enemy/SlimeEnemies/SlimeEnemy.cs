
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Enemy.SlimeEnemies.Components;
using LSW._02._Scripts.Entity.Enemy.SlimeEnemies.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies
{
    public class SlimeEnemy : BaseEnemy
    {
        [Header("Attack Probability")] 
        [Range(0f, 1f)] [SerializeField] private float whirlwindAttackChance = 0.2f;

        [Header("Attack Decision")]
        [SerializeField] private float attackDecisionInterval = 1f;
        
        private SlimeCombatComponent _slimeCombat;
        private SlimeWhirlwindHitboxComponent _whirlwindHitbox;
        private SlimeAnimation _slimeAnimation;

        private float _decisionTimer;

        public SlimeCombatComponent SlimeCombat => _slimeCombat;
        public SlimeAnimation SlimeAnimation => _slimeAnimation;
        
        public const string IdleState = "SlimeIdleState";
        public const string ChaseState = "SlimeChaseState";
        public const string FrontAttackState = "SlimeFrontAttackState";
        public const string WhirlwindState = "SlimeWhirlwindState";

        protected override void Awake()
        {
            base.Awake();

            CreateStateMachine();
        }

        private void Start()
        {
            if(Animation != null && Animation is SlimeAnimation slimeAnimation)
                _slimeAnimation = slimeAnimation;
            GetCompo(out _slimeCombat);
            GetCompo(out _whirlwindHitbox);
            
            BindAnimationEvents();
            
            StateMachine.Initialize(IdleState);
            Spawn(); // ¾ø¾Ù ¿¹Á¤
        }

        private void CreateStateMachine()
        {
            StateMachine = new EntityStateMachine(
                    new List<StateData>
                    {
                        new StateData
                        {
                            stateName = IdleState,
                            State = new SlimeIdleState(this)
                        },
                        new StateData
                        {
                            stateName = ChaseState,
                            State = new SlimeChaseState(this)
                        },
                        new StateData
                        {
                            stateName = FrontAttackState,
                            State = new SlimeFrontAttackState(this)
                        },
                        new StateData
                        {
                            stateName = WhirlwindState,
                            State = new SlimeWhirlwindState(this)
                        }
                    }
                );
        }

        public override void Spawn()
        {
            base.Spawn();

            _decisionTimer = 0f;

            if (_whirlwindHitbox != null)
                _whirlwindHitbox.gameObject.SetActive(false);
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
            if (_slimeCombat == null)
                return false;

            if (_decisionTimer > 0f)
                return false;

            _decisionTimer =
                attackDecisionInterval;

            float random =
                Random.value;
            
            if (random < whirlwindAttackChance && _slimeCombat.CanWhirlwindAttack)
            {
                ChangeState(WhirlwindState);
                return true;
            }
            
            if (_slimeCombat.CanFrontAttack)
            {
                ChangeState(FrontAttackState);
                return true;
            }

            return false;
        }

        public void ChangeState(string stateName)
        {
            if (StateMachine == null)
                return;

            StateMachine.ChangeState(stateName);
        }

        public void SetWhirlwindHitbox(bool active)
        {
            if (_whirlwindHitbox == null)
                return;

            _whirlwindHitbox.gameObject.SetActive(active);
        }

        #region Animation Events

        private void BindAnimationEvents()
        {
            if (_slimeAnimation == null)
                return;
            
            _slimeAnimation.OnFrontAttackCast += HandleFrontAttackCast;
            _slimeAnimation.OnFrontAttackAnimationEnd += HandleFrontAttackEnd;
            _slimeAnimation.OnWhirlwindAttackCast += HandleWhirlwindCast;
            _slimeAnimation.OnWhirlwindAnimationEnd += HandleWhirlwindEnd;
        }

        private void UnbindAnimationEvents()
        {
            if (_slimeAnimation == null)
                return;
            
            _slimeAnimation.OnFrontAttackCast -= HandleFrontAttackCast;
            _slimeAnimation.OnFrontAttackAnimationEnd -= HandleFrontAttackEnd;
            _slimeAnimation.OnWhirlwindAttackCast -= HandleWhirlwindCast;
            _slimeAnimation.OnWhirlwindAnimationEnd -= HandleWhirlwindEnd;
        }

        private void HandleFrontAttackCast()
        {
            if (StateMachine.CurrentState?.stateName !=
                FrontAttackState)
            {
                return;
            }

            if (Player == null)
                return;

            Vector2 direction = (Player.transform.position -
                                 transform.position).normalized;

            _slimeCombat.TryFrontAttack(Player, direction);
        }

        private void HandleFrontAttackEnd()
        {
            if (StateMachine.CurrentState?.State
                is not SlimeFrontAttackState state)
            {
                return;
            }
            
            state.AnimationEnd();
        }

        private void HandleWhirlwindCast()
        {
            if (StateMachine.CurrentState?.State
                is not SlimeWhirlwindState state)
            {
                return;
            }

            state.AttackCast();
        }

        private void HandleWhirlwindEnd()
        {
            if (StateMachine.CurrentState?.State is not SlimeWhirlwindState state)
            {
                return;
            }

            state.AnimationEnd();
        }

        #endregion

        protected override void OnDeath()
        {
            SetWhirlwindHitbox(false);

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
            SetWhirlwindHitbox(false);
        }
    }
}