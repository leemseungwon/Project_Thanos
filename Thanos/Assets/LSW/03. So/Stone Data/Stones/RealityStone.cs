using System.Collections.Generic;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "RealityStone", menuName = "So/Data/Stone Data/Reality Stone", order = 0)]
    public class RealityStone : BaseStone
    {
        [Header("Condition")]
        [SerializeField] private int requiredParryCount = 3;

        [Header("Ability")]
        [SerializeField] private float range = 6f;
        [SerializeField] private LayerMask enemyLayer;

        public int RequiredParryCount => requiredParryCount;

        public override void Use(PlayerController owner)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                owner.transform.position,
                range,
                enemyLayer
            );

            List<BaseEntity> enemies = new();

            foreach (Collider2D hit in hits)
            {
                BaseEntity entity =
                    hit.GetComponentInParent<BaseEntity>();

                if (entity == null)
                    continue;

                if (entity == owner)
                    continue;

                if (enemies.Contains(entity))
                    continue;

                enemies.Add(entity);
            }

            if (enemies.Count < 2)
                return;

            Vector3[] positions = new Vector3[enemies.Count];

            for (int i = 0; i < enemies.Count; i++)
            {
                positions[i] = enemies[i].transform.position;
            }
            
            for (int i = 0; i < enemies.Count; i++)
            {
                int nextIndex = (i + 1) % enemies.Count;

                enemies[i].transform.position = positions[nextIndex];
            }
        }
    }
}