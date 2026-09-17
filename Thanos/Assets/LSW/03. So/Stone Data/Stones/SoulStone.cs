using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "SoulStone", menuName = "So/Data/Stone Data/Soul Stone", order = 0)]
    public class SoulStone : BaseStone
    {
        [Header("Condition")]
        [SerializeField] private int requiredKillCount = 5;

        [Header("Ability")]
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private AttackData attackData;
        [SerializeField] private LayerMask enemyLayer;

        public int RequiredKillCount =>
            requiredKillCount;

        public override void Use(PlayerController owner)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                owner.transform.position,
                explosionRadius,
                enemyLayer
            );

            foreach (Collider2D hit in hits)
            {
                BaseEntity enemy =
                    hit.GetComponentInParent<BaseEntity>();

                if (enemy == null || enemy == owner)
                    continue;
                
                Vector2 dir = (hit.transform.position - owner.transform.position).normalized;
                
                enemy.TakeDamage(attackData.attackDamage + owner.CurrentAttackDamage, dir, attackData.knockbackPower, attackData.stunDuration, owner);
            }
        }
    }
}