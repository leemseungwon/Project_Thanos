
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Environment.Stone_Ability;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Power_Stone
{
    [CreateAssetMenu(fileName = "Power Laser", menuName = "So/Data/Stone Data/Stone Upgrade/Power Stone Upgrade/Power Laser", order = 0)]
    public class PowerLaserUpgrade : BaseStoneUpgrade
    {
        [Header("Laser")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private float range = 5f;
        [SerializeField] private float knockbackPower = 0f;

        [Header("Pool")]
        [SerializeField] private string laserPoolName = "PowerLaser";

        [Header("Target")]
        [SerializeField] private LayerMask enemyLayer;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null)
                return;

            if (!SystemHandler.Instance.GetSystem(out PoolSystem poolSystem))
            {
                return;
            }

            Vector2[] directions = { 
                Vector2.up, Vector2.right,
                Vector2.down, Vector2.left
            };

            int finalDamage = Mathf.Max(1, 
                Mathf.RoundToInt(damage + player.CurrentAttackDamage));

            foreach (Vector2 direction in directions)
            {
                if (!poolSystem.SpawnPool(laserPoolName, out PowerLaser laser))
                {
                    continue;
                }
                laser.Setup(player, direction, range,
                    finalDamage, knockbackPower, enemyLayer);
            }
        }
    }
}
