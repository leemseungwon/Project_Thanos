using System;
using LSW._02._Scripts.Entity.Enemy.Components;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data.Enemy;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.Rat_Enemys.Components
{
    public class RatEnemyCombatComponent : EnemyCombatComponent
    {
        [Header("Attack")]
        [SerializeField] private EnemyAttackData attackData;
        [SerializeField] private float attackOffset = 0.8f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private LayerMask playerLayer;

        private RatEnemy _enemy;
        private RatEnemyAnimation _animation;
        
        private float _cooldownTimer;

        public bool CanAttack => _cooldownTimer <= 0f;
        
        public event Action OnAttackStarted;
        public event Action OnAttackFinished;

        public override void Initialize(BaseEntity owner)
        {
            base.Initialize(owner);

            _enemy = owner as RatEnemy;

            if (_enemy != null)
            {
                _enemy.GetCompo(out _animation);

                if (_animation != null)
                {
                    _animation.OnAttackCast += HandleAttackCast;
                    _animation.OnAttackAnimationEnd += HandleAttackAnimationEnd;
                }
            }
        }
        
        private void HandleAttackCast()
        {
            if (_enemy == null)
                return;

            if (!CanAttack)
                return;

            int finalDamage = _enemy.CurrentAttackDamage + attackData.attackDamage;
            Vector2 attackPosition = (Vector2)_enemy.transform.position +
                                     _enemy.LookDirection * attackOffset;

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition,
                attackData.attackRadius, playerLayer);

            foreach (Collider2D hit in hits)
            {
                PlayerController target =
                    hit.GetComponentInParent<PlayerController>();

                if (target == null)
                    continue;

                target.TakeDamage(finalDamage, _enemy.LookDirection, 0,
                    0, _enemy);
                
                break;
            }

            _cooldownTimer = attackCooldown;
        }

        private void HandleAttackAnimationEnd()
        {
            float cooldownMultiplier = 1f;
            if (_enemy.GetCompo(out EntityBuffComponent buff))
            {
                cooldownMultiplier = buff.GetAttackCooldownMultiplier();
            }

            _enemy.Movement.EnableMovement(true);
            _cooldownTimer = attackCooldown * cooldownMultiplier;
            
            OnAttackFinished?.Invoke();
        }

        private void Update()
        {
            UpdateCooldown();
        }

        private void UpdateCooldown()
        {
            if (_cooldownTimer <= 0f)
                return;

            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer < 0f)
                _cooldownTimer = 0f;
        }

        public override bool TryAttack(PlayerController player)
        {
            if (!CanAttack)
                return false;
            
            _enemy.Movement.EnableMovement(false);
            _animation.SetParam(_animation.AttackAnimationData);

            OnAttackStarted?.Invoke();
            
            return true;
        }

        public override void Reset()
        {
            _cooldownTimer = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            if (attackData == null)
                return;

            Vector2 attackPosition = _enemy != null ?
                (Vector2)_enemy.transform.position + _enemy.LookDirection * attackOffset : 
                (Vector2)transform.position + -(Vector2)transform.up * attackOffset;

            Gizmos.DrawWireSphere(attackPosition, attackData.attackRadius);
        }
    }
}