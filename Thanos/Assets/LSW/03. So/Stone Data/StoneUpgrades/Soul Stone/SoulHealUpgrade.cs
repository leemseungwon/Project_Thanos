using System.Collections.Generic;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Soul_Stone
{
    [CreateAssetMenu(fileName = "Soul Heal", menuName = "So/Data/Stone Data/Stone Upgrade/Soul Stone Upgrade/Soul Heal", order = 0)]
    public class SoulHealUpgrade : BaseStoneUpgrade
    {
        [SerializeField]
        private float healPerEnemy = 3f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null || stone is not SoulStone soulStone ||
                healPerEnemy <= 0f)
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position,
                soulStone.Radius, soulStone.EnemyLayer);
            HashSet<BaseEnemy> affectedEnemies = new();

            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponentInParent<BaseEnemy>();
                if (enemy != null && !enemy.IsDead)
                    affectedEnemies.Add(enemy);
            }

            int healAmount = Mathf.RoundToInt(healPerEnemy * affectedEnemies.Count);
            player.Heal(healAmount);
        }
    }
}