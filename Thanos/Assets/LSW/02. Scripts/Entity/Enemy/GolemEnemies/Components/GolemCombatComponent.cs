
using LSW._02._Scripts.Entity.Enemy.Components;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using LSW._03._So.Attack_Data.Enemy;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies.Components
{
    public class GolemCombatComponent : EnemyCombatComponent
    {
        [SerializeField] private LayerMask playerLayer;
        
        [Header("Front Attack")]
        [SerializeField] private EnemyAttackData frontAttackData;
        [SerializeField] private EnemyHitPoint frontAttackHitPoint;

        [Header("Clap Attack")]
        [SerializeField] private EnemyAttackData clapAttackData;
        [SerializeField] private EnemyHitPoint clapHitPoint;

        private PoolSystem _poolSystem;
        
        private float _frontCooldown;
        private float _clapCooldown;

        public bool CanFrontAttack => _frontCooldown <= 0f && (frontAttackHitPoint != null && frontAttackHitPoint.IsInHitPoint);
        public bool CanClapAttack => _clapCooldown <= 0f && (clapHitPoint != null && clapHitPoint.IsInHitPoint);

        private void Awake()
        {
            VLog.LogTerm(SystemHandler.Instance.GetSystem(out _poolSystem), LogType.Error);
        }

        public void ApplyFrontAttackCooldown() => _frontCooldown = frontAttackData.attackCoolDown;
        public void ApplyClapCooldown() => _clapCooldown = clapAttackData.attackCoolDown;

        public override void Initialize(BaseEntity owner)
        {
            base.Initialize(owner);
            frontAttackHitPoint.Initialize(frontAttackData.attackRadius);
            clapHitPoint.Initialize(clapAttackData.attackRadius);
        }

        private void Update()
        {
            if (Owner == null)
                return;

            float deltaTime = Time.deltaTime;
            _frontCooldown = Mathf.Max(0f, _frontCooldown - deltaTime);
            _clapCooldown = Mathf.Max(0f, _clapCooldown - deltaTime);
        }

        public override bool TryAttack(PlayerController player)
        {
            if (player == null)
                return false;

            // 로직상 GolemEnemy.TryChooseAttack에서 처리하므로 여기서는 기본값 반환
            return false;
        }

        public void PerformFrontAttack()
        {
            if (frontAttackData == null || Owner == null) 
                return;

            Vector2 direction = Owner.transform.right;

            Vector2 origin = Owner.Rigidbody.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, frontAttackData.attackRadius, playerLayer);
            if (hit.collider != null && hit.collider.TryGetComponent(out PlayerController player))
            {
                ApplyAttack(player, frontAttackData, direction);
            }
        }

        public void PerformClapAttack()
        {
            if (clapAttackData == null) 
                return;
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(Owner.transform.position, clapAttackData.attackRadius, playerLayer);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out PlayerController player))
                {
                    Vector2 direction = (player.transform.position - Owner.transform.position).normalized;
                    ApplyAttack(player, clapAttackData, direction);
                }
            }
        }

        
        private void ApplyAttack(PlayerController player, EnemyAttackData attackData, Vector2 direction)
        {
            if (player == null || attackData == null)
                return;

            int finalDamage = attackData.attackDamage + Owner.CurrentAttackDamage;

            player.TakeDamage(finalDamage, direction, attackData.knockbackPower,
                0f, Owner, attackData);
        }

        public override void Reset()
        {
            _frontCooldown = 0f;
            _clapCooldown = 0f;
        }
    }
}
