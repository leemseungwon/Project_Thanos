using System;
using System.Collections.Generic;
using LSW._03._So.Attack_Data.Player;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerCombatComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Attack")]
        [SerializeField] private PlayerAttackData normalAttackData;
        [SerializeField] private float normalAttackCooldown = 0.2f;

        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackOffset = 1f;
        [SerializeField] private LayerMask enemyLayer;
        
        private PlayerController _owner;
        private PlayerAnimation _animation;
        
        private Vector2 _attackDirection;
        private float _cooldownTimer;

        public bool IsAttacking { get; private set; }
        
        public bool CanAttack => _cooldownTimer <= 0f;

        public event Action OnAttackStarted;
        public event Action OnAttackFinished;

        public void Initialize(BaseEntity owner)
        {
            _owner = owner as PlayerController;

            if (_owner != null)
            {
                _owner.GetCompo(out _animation);

                if (_animation != null)
                {
                    _animation.OnAttackCast += HandleAttackCast;
                    _animation.OnAttackAnimationEnd += HandleAttackAnimationEnd;
                }
            }
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

        public bool TryAttack()
        {
            if (!CanAttack)
                return false;
            
            _owner.EnableInput(false);
            _attackDirection = Get4Direction(_owner.LookDirection);
            _animation.SetParam(_animation.AttackAnimationData);

            OnAttackStarted?.Invoke();
            IsAttacking = true;
            
            _owner.Stone.OnPlayerAttack();
            
            return true;
        }
        
        private Vector2 Get4Direction(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x > 0f ? Vector2.right : Vector2.left;
            }
            else
            {
                return direction.y > 0f ? Vector2.up : Vector2.down;
            }
        }

        private void HandleAttackCast()
        {
            NormalAttack();
        }

        private void HandleAttackAnimationEnd()
        {
            _owner.EnableInput(true);
            IsAttacking = false;
            
            float cooldownMultiplier = 1f;
            if (_owner.GetCompo(out EntityBuffComponent buff))
            {
                cooldownMultiplier = buff.GetAttackCooldownMultiplier();
            }

            _cooldownTimer = normalAttackCooldown * cooldownMultiplier;
            
            OnAttackFinished?.Invoke();
        }

        public void NormalAttack()
        {
            if (_owner == null)
                return;

            if (normalAttackData == null)
                return;

            int finalDamage = normalAttackData.attackDamage + _owner.CurrentAttackDamage;
            if (_owner.GetCompo(out EntityBuffComponent buff))
            { 
                finalDamage = buff.ModifyOutgoingDamage(finalDamage);
            }

            if (_owner.Stone != null)
                finalDamage = _owner.Stone.ApplyComboDamageBonus(finalDamage);
            
            Attack(finalDamage, normalAttackData.knockbackPower, normalAttackData.stunDuration);
        }

        public void Attack(int damage, float knockBackPower, float stunDuration)
        {
            if (_owner == null)
                return;

            Vector2 attackPosition = (Vector2)_owner.transform.position 
                                     + _attackDirection * attackOffset;

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange,
                enemyLayer);
            HashSet<IDamageable> damagedTargets = new();
            bool hitDamageable = false;

            foreach (Collider2D hit in hits)
            {
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable == null)
                    continue;

                if (!damagedTargets.Add(damageable))
                    continue;

                Vector2 knockBackDir = (hit.transform.position - _owner.transform.position).normalized;

                hitDamageable |= damageable.TakeDamage(damage, knockBackDir, knockBackPower, stunDuration, _owner);
            }
            
            if (hitDamageable)
                _owner.Stone.OnEnemyHit();
        }
        
        public float GetCooldownRemaining()
        {
            return Mathf.Max(0f, _cooldownTimer);
        }
        
        public void Reset() { }

        private void OnDestroy()
        {
            if (_animation != null)
            {
                _animation.OnAttackCast -= HandleAttackCast;
                _animation.OnAttackAnimationEnd -= HandleAttackAnimationEnd;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 direction = Application.isPlaying ? _attackDirection
                : Vector2.right;

            if (direction.sqrMagnitude < 0.001f)
                direction = Vector2.right;

            direction.Normalize();

            Vector2 attackPosition =
                (Vector2)transform.position + direction * attackOffset;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPosition, attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, attackPosition);
        }
    }
}
