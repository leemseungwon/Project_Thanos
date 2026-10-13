using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Enemy.Rat_Enemys.States;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.Rat_Enemys
{
    public class RatEnemy : BaseEnemy
    {
        [SerializeField] private string ratEnemyPoolName = "Enemy_Rat";
        
        public Vector2 LookDirection { get; private set; } = Vector2.down;
        public RatEnemyAnimation RatAnimation { get; private set; }
        public bool IsAttacking { get; set; }
        
        public const string ChaseState = "RatChaseState";
        public const string AttackState = "RatAttackState";

        public override void Initialize()
        {
            base.Initialize();

            RatAnimation = Animation as RatEnemyAnimation;
            
            InitState();
        }

        private void InitState()
        {
            StateMachine = new EntityStateMachine
            (
                new List<StateData>{
                    new StateData
                    {
                        stateName = ChaseState,
                        State = new RatEnemyChaseState(this)
                    },
                    new StateData
                    {
                        stateName = AttackState,
                        State = new RatEnemyAttackState(this)
                    },
                }
            );
        }

        public override void Spawn()
        {
            base.Spawn();

            LookDirection = Vector2.down;

            StateMachine.Initialize(ChaseState);
        }

        protected override void Update()
        {
            base.Update();

            if (IsDead)
                return;

            UpdateLookDirection();
        }

        private void UpdateLookDirection()
        {
            if (Target == null || IsStunned)
                return;

            Vector2 direction = Target.transform.position - transform.position;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            LookDirection = direction.normalized;
            
            RatAnimation.SetParam(RatAnimation.LookDirectionXAnimationData, LookDirection.x);
            RatAnimation.SetParam(RatAnimation.LookDirectionYAnimationData, LookDirection.y);
        }

        public void AnimationAttackFinished()
        {
            FinishAttack();
        }

        public void FinishAttack()
        {
            if (IsDead)
                return;

            StateMachine.ChangeState(ChaseState);
        }

        public override bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower, float stunDuration,
            BaseEntity attacker)
        {
            Vector2 hitDir = (transform.position - attacker.transform.position).normalized;
            
            if (RatAnimation != null && !IsAttacking)
            {
                RatAnimation.SetParam(RatAnimation.IsMoveAnimationData, false);
                RatAnimation.SetParam(RatAnimation.GetHitDirXAnimationData, hitDir.x);
                RatAnimation.SetParam(RatAnimation.GetHitDirYAnimationData, hitDir.y);
                RatAnimation.SetParam(RatAnimation.GetHitAnimationData);
            }
            
            return base.TakeDamage(finalDamage, knockBackDirection, knockBackPower, stunDuration, attacker);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            Movement.Stop();
            if (SystemHandler.Instance.GetSystem(out PoolSystem poolSystem))
            {
                poolSystem.DespawnPool(ratEnemyPoolName, this);
            }
        }
    }
}