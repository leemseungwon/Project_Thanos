using System.Collections.Generic;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Power_Stone
{
    [CreateAssetMenu(fileName = "Power Dot", menuName = "So/Data/Stone Data/Stone Upgrade/Power Stone Upgrade/Power Dot", order = 0)]
    public class PowerDotUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float duration = 3f;
        [SerializeField] private float damagePerSecond = 3f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null)
                return;

            if (stone is not PowerStone powerStone)
                return;

            if (damagePerSecond <= 0f || duration <= 0f)
            {
                return;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position,
                powerStone.Radius, powerStone.EnemyLayer);

            HashSet<BaseEnemy> affected = new();

            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponentInParent<BaseEnemy>();

                if (enemy == null)
                    continue;

                if (enemy.IsDead || !affected.Add(enemy))
                    continue;

                if (!enemy.GetCompo(out EntityBuffComponent buff))
                {
                    continue;
                }

                int finalDamagePerTick = Mathf.Max(1,
                    Mathf.RoundToInt(damagePerSecond + player.CurrentAttackDamage));
                buff.ApplyDamageOverTime("PowerStone_Dot", player, finalDamagePerTick,
                    1f, duration);
            }
        }
    }
}