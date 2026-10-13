
using System.Collections.Generic;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Environment.Particle;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "PowerStone", menuName = "So/Data/Stone Data/Stones/Power Stone", order = 0)]
    public class PowerStone : BaseStone
    {
        [Header("Power Stone")]
        [SerializeField] private float radius = 2f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float knockbackPower = 1f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private string abilityParticleName = "Particle_PowerAbility";

        public float Radius => radius;
        public LayerMask EnemyLayer => enemyLayer;

        public override void Use(PlayerController owner)
        {
            if (owner == null)
                return;

            if (SystemHandler.Instance.GetSystem(out PoolSystem poolSystem))
            {
                if(poolSystem.SpawnPool(abilityParticleName, out PoolingParticle particle))
                {
                    particle.transform.position = owner.transform.position;
                }
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(owner.transform.position,
                radius, enemyLayer);

            HashSet<BaseEnemy> damaged = new();

            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponentInParent<BaseEnemy>();

                if (enemy == null)
                    continue;

                if (enemy.IsDead || !damaged.Add(enemy))
                    continue;

                Vector2 direction = (enemy.transform.position -
                                     owner.transform.position).normalized;

                enemy.TakeDamage(damage + owner.CurrentAttackDamage, direction,
                    knockbackPower, 0f, owner);
            }
        }

        private void OnValidate()
        {
            radius = Mathf.Max(0f, radius);
            damage = Mathf.Max(0, damage);
            knockbackPower = Mathf.Max(0f, knockbackPower);
        }
    }
}