
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Environment.Particle;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "SoulStone", menuName = "So/Data/Stone Data/Stones/Soul Stone", order = 0)]
    public class SoulStone : BaseStone
    {
        [Header("Soul Stone")]
        [SerializeField] private float radius = 3f;
        [SerializeField] private float moveSpeedMultiplier = 0.5f;
        [SerializeField] private float attackDamageMultiplier = 0.5f;
        [SerializeField] private float duration = 5f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private string abilityParticleName = "Particle_SoulAbility";

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
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(owner.transform.position, radius, enemyLayer);

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out BaseEnemy enemy))
                    continue;

                if (!enemy.GetCompo(out EntityBuffComponent buff))
                    continue;

                buff.ApplyMoveSpeedMultiplier("SoulStone_Slow",
                    moveSpeedMultiplier, duration);

                buff.ApplyAttackDamageMultiplier("SoulStone_DamageDown",
                    attackDamageMultiplier, duration);
            }
        }
    }
}