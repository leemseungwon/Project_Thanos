using LSW._02._Scripts.Entity.Enemy.Components;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies.Components
{
    public class SlimeCombatComponent : EnemyCombatComponent
    {
        [Header("Front Attack")]
        [SerializeField] private AttackData frontAttackData;
        [SerializeField] private float frontAttackRange = 1.5f;
        [SerializeField] private float frontAttackCooldown = 1f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Jump Attack")]
        [SerializeField] private AttackData jumpAttackData;
        [SerializeField] private float jumpAttackRange = 4f;
        [SerializeField] private float jumpAttackCooldown = 2.5f;

        [Header("Whirlwind Attack")]
        [SerializeField] private AttackData whirlwindAttackData;
        [SerializeField] private float whirlwindAttackRange = 3f;
        [SerializeField] private float whirlwindAttackCooldown = 3f;

        private float _frontCooldown;
        private float _jumpCooldown;
        private float _whirlwindCooldown;
        
        public bool CanFrontAttack => _frontCooldown <= 0f;
        public bool CanWhirlwindAttack => _whirlwindCooldown <= 0f;
        
        public AttackData WhirlwindAttackData => whirlwindAttackData;

        private void Update()
        {
            if (Owner == null)
                return;

            _frontCooldown = Mathf.Max(
                0f,
                _frontCooldown - Time.deltaTime);

            _jumpCooldown = Mathf.Max(
                0f,
                _jumpCooldown - Time.deltaTime);

            _whirlwindCooldown = Mathf.Max(
                0f,
                _whirlwindCooldown - Time.deltaTime);
        }

        public override bool TryAttack(PlayerController player)
        {
            if (player == null)
                return false;

            Vector2 direction =
                (player.transform.position - Owner.transform.position)
                .normalized;

            return TryFrontAttack(player, direction);
        }

        public bool TryFrontAttack(PlayerController player, Vector2 direction)
        {
            if (!CanFrontAttack)
                return false;

            if (!CanDamagePlayer(player))
                return false;

            if (frontAttackData == null)
                return false;

            direction.Normalize();

            Vector2 origin = Owner.Rigidbody.position;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction,
                frontAttackRange, playerLayer);

            if (hit.collider == null)
                return false;

            if (hit.collider.TryGetComponent(out PlayerController playerController))
            {
                _frontCooldown = frontAttackCooldown;

                ApplyAttack(playerController, frontAttackData, direction);
                return true;
            }

            return false;
        }
        
        private void ApplyAttack(PlayerController player, AttackData attackData, Vector2 direction)
        {
            if (player == null || attackData == null)
                return;

            int finalDamage = attackData.attackDamage + Owner.CurrentAttackDamage;

            player.TakeDamage(finalDamage, direction, attackData.knockbackPower,
                attackData.stunDuration, Owner);
        }

        private bool CanDamagePlayer(PlayerController player)
        {
            return player != null && !player.IsDead && !player.IsStunned;
        }
        
        public bool CanHitWithFrontAttack(Vector2 direction)
        {
            if (Owner == null)
                return false;
            
            direction.Normalize();

            RaycastHit2D hit = Physics2D.Raycast(Owner.Rigidbody.position, direction,
                frontAttackRange, playerLayer);

            if (hit.collider == null)
                return false;
            
            if (hit.collider.TryGetComponent(out PlayerController _))
            {
                return true;
            }
            return false;
        }

        public bool IsInFrontAttackRange(Vector2 position)
        {
            return IsInRange(position, frontAttackRange);
        }

        private bool IsInRange(Vector2 targetPosition, float range)
        {
            if (Owner == null)
                return false;

            return Vector2.Distance(Owner.transform.position,
                       targetPosition) <= range;
        }

        public override void Reset()
        {
            _frontCooldown = 0f;
            _jumpCooldown = 0f;
            _whirlwindCooldown = 0f;
        }
    }
}