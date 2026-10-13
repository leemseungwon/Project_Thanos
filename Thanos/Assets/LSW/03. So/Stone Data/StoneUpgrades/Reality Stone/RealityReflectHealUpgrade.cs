using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Reality_Stone
{
    [CreateAssetMenu(fileName = "Reality Reflect Heal", menuName = "So/Data/Stone Data/Stone Upgrade/Reality Stone Upgrade/Reality Reflect Heal", order = 0)]
    public class RealityReflectHealUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private int healAmount = 5;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null)
                return;

            if (!player.GetCompo(out EntityBuffComponent buff))
                return;

            buff.EnhanceShield(reflectHeal: true,
                reflectHealAmount: 5);
        }
    }
}