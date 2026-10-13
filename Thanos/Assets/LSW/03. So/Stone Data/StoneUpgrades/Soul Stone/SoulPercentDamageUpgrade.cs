using System.Collections.Generic;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Soul_Stone
{
    [CreateAssetMenu(fileName = "Soul Percent Damage", menuName = "So/Data/Stone Data/Stone Upgrade/Soul Stone Upgrade/Soul Percent Damage", order = 0)]
    public class SoulPercentDamageUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float maxHealthPercent = 0.1f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null || stone is not SoulStone soulStone ||
                maxHealthPercent <= 0f)
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position,
                soulStone.Radius, soulStone.EnemyLayer);
            HashSet<BaseEnemy> damagedEnemies = new();

            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponentInParent<BaseEnemy>();
                if (enemy == null || enemy.IsDead || !damagedEnemies.Add(enemy))
                    continue;

                int damage = Mathf.Max(1, Mathf.RoundToInt(enemy.MaxHp * maxHealthPercent));
                enemy.TakeDamage(damage, Vector2.zero, 0f, 0f, player);
            }
        }
    }
}