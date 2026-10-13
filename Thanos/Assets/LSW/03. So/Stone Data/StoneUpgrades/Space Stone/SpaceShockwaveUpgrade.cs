
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data.Player;
using System.Collections.Generic;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Space_Stone
{
    [CreateAssetMenu(fileName = "Space Shockwave Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Space Stone Upgrade/Space Shockwave", order = 0)]
    public class SpaceShockwaveUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private PlayerAttackData shockwaveAttackData;
        [SerializeField] private float radius = 2f;
        [SerializeField] private LayerMask enemyLayer;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                    player.transform.position, radius, enemyLayer);
            HashSet<BaseEnemy> damagedEnemies = new();

            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponentInParent<BaseEnemy>();
                if (enemy == null || enemy.IsDead || !damagedEnemies.Add(enemy))
                    continue;

                int finalDamage = player.CurrentAttackDamage + shockwaveAttackData.attackDamage;
                Vector2 knockbackDir = (hit.transform.position - player.transform.position).normalized;

                enemy.TakeDamage(finalDamage, knockbackDir, shockwaveAttackData.knockbackPower,
                    shockwaveAttackData.stunDuration, player);
            }
        }
    }
}
