using System;
using System.Collections.Generic;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerCombatComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Attack")]
        [SerializeField] private List<AttackData> normalAttackDataList;

        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private LayerMask enemyLayer;

        [Header("Combo")]
        [SerializeField] private int maxComboCount = 4;
        [SerializeField] private float comboInputWindow = 0.3f;
        [SerializeField] private float successComboAttackCooldown = 1f;
        [SerializeField] private float failComboAttackCooldown = 0.2f;

        private PlayerController _owner;
        private PlayerAnimation _animation;

        private float _comboTimer;
        private float _cooldownTimer;
        private bool _isAttacking;
        private bool _isWaitingForNextAttack;
        private bool _nextAttackQueued;

        public bool IsAttacking => _isAttacking;
        public bool IsWaitingForNextAttack => _isWaitingForNextAttack;
        public bool CanAttack => _cooldownTimer <= 0f;
        public int ComboCount { get; private set; }

        public event Action<int> OnAttackStarted;
        public event Action OnAttackFinished;
        public event Action OnComboFinished;

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
            UpdateComboWindow();
        }

        private void UpdateCooldown()
        {
            if (_cooldownTimer <= 0f)
                return;

            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer < 0f)
                _cooldownTimer = 0f;
        }

        private void UpdateComboWindow()
        {
            if (!_isWaitingForNextAttack)
                return;

            _comboTimer -= Time.deltaTime;

            if (_comboTimer <= 0f)
            {
                FinishCombo();
            }
        }

        public bool TryAttack()
        {
            if (!CanAttack)
                return false;
            
            if (_isAttacking)
            {
                if (ComboCount >= maxComboCount)
                    return false;

                _nextAttackQueued = true;
                return true;
            }
            
            if (_isWaitingForNextAttack)
            {
                if (ComboCount >= maxComboCount)
                    return false;

                _isWaitingForNextAttack = false;
                _comboTimer = 0f;
                ComboCount++;
                _isAttacking = true;
                _nextAttackQueued = false;

                PlayAttackAnimation(ComboCount);

                return true;
            }

            ComboCount = 1;

            _isAttacking = true;
            _isWaitingForNextAttack = false;
            _nextAttackQueued = false;

            PlayAttackAnimation(ComboCount);

            return true;
        }

        private void PlayAttackAnimation(int combo)
        {
            if (_animation == null)
                return;

            OnAttackStarted?.Invoke(combo);

            switch (combo)
            {
                case 1:
                    _animation.SetParam(_animation.Attack1AnimationData);
                    break;

                case 2:
                    _animation.SetParam(_animation.Attack2AnimationData);
                    break;

                case 3:
                    _animation.SetParam(_animation.Attack3AnimationData);
                    break;

                case 4:
                    _animation.SetParam(_animation.Attack4AnimationData);
                    break;
            }
        }

        private void HandleAttackCast()
        {
            if (!_isAttacking)
                return;

            NormalAttack();
        }

        private void HandleAttackAnimationEnd()
        {
            if (!_isAttacking)
                return;
            
            _isAttacking = false;
            OnAttackFinished?.Invoke();
            
            if (ComboCount >= maxComboCount)
            {
                FinishCombo();
                return;
            }

            if (_nextAttackQueued)
            {
                _nextAttackQueued = false;
                ComboCount++;
                _isAttacking = true;
                
                PlayAttackAnimation(ComboCount);
                return;
            }

            _isWaitingForNextAttack = true;
            _comboTimer = comboInputWindow;
        }

        private void FinishCombo()
        {
            bool isSuccess = ComboCount >= maxComboCount;
            
            _isAttacking = false;
            _isWaitingForNextAttack = false;
            _nextAttackQueued = false;
            _comboTimer = 0f;
            ComboCount = 0;

            if (isSuccess)
                _cooldownTimer = successComboAttackCooldown;
            else
                _cooldownTimer = failComboAttackCooldown;

            OnComboFinished?.Invoke();
        }

        public void NormalAttack()
        {
            if (_owner == null)
                return;

            if (normalAttackDataList == null)
                return;

            int index = ComboCount - 1;

            if (index < 0 || index >= normalAttackDataList.Count)
            {
                return;
            }

            AttackData attackData = normalAttackDataList[index];

            if (attackData == null)
                return;

            int finalDamage = attackData.attackDamage + _owner.CurrentAttackDamage;

            Attack(finalDamage, attackData.knockbackPower, attackData.stunDuration);
        }

        public void Attack(int damage, float knockBackPower, float stunDuration)
        {
            if (_owner == null || attackPoint == null)
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position,
                attackRange, enemyLayer);

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out IDamageable damageable))
                    continue;

                Vector2 knockBackDir = (hit.transform.position - _owner.transform.position).normalized;

                damageable.TakeDamage(damage, knockBackDir, knockBackPower,
                    stunDuration, _owner);
            }
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
            if (attackPoint == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}