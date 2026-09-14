using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerCombatComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Attack")] 
        [SerializeField] private AttackData normalAttackData;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private Transform attackPoint;

        private PlayerController _owner;
        
        public void Initialize(BaseEntity owner)
        {
            _owner = owner as PlayerController;
        }
        
        public void Attack(Vector2 mousePosition)
        {
            Vector3 worldPosition =
                _owner.PlayerCamera.ScreenToWorldPoint(mousePosition);

            Vector2 direction =
                (worldPosition - transform.position).normalized;

            if (direction.sqrMagnitude <= 0.01f)
                return;

            attackPoint.right = direction;

            RaycastHit2D hit = Physics2D.Raycast(
                attackPoint.position,
                direction,
                attackRange,
                normalAttackData.enemyLayer);

            if (hit.collider == null)
                return;

            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(normalAttackData, 
                    normalAttackData.attackDamage + _owner.CurrentAttackDamage, _owner);
            }
        }
    }
}