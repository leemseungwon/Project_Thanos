using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "PowerStone", menuName = "So/Data/Stone Data/Power Stone", order = 0)]
    public class PowerStone : BaseStone
    {
        [Header("Condition")]
        [SerializeField] private int requiredDamage = 100;

        [Header("Ability")]
        [SerializeField] private AttackData attackData;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float range = 4f;
        [SerializeField] private float angle = 60f;

        public int RequiredDamage => requiredDamage;

        public override void Use(PlayerController owner)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                owner.transform.position,
                range,
                enemyLayer
            );

            Vector2 forward = owner.transform.right;

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out IDamageable damageable))
                {
                    return;
                }
                
                Vector2 direction =
                    hit.transform.position -
                    owner.transform.position;

                if (Vector2.Angle(forward, direction) > angle * 0.5f)
                    continue;

                damageable.TakeDamage(attackData.attackDamage + owner.CurrentAttackDamage, direction,
                    attackData.knockbackPower, attackData.stunDuration, owner);
            }
        }
    }
}