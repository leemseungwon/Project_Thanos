using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Reality_Stone
{
    [CreateAssetMenu(fileName = "Reality Chain Reflect", menuName = "So/Data/Stone Data/Stone Upgrade/Reality Stone Upgrade/Reality Chain Reflect", order = 0)]
    public class RealityChainReflectUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float radius = 3f;
        [SerializeField] private LayerMask enemyLayer;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null)
                return;

            if (!player.GetCompo(out EntityBuffComponent buff))
                return;

            buff.EnhanceShield(chainReflect: true,
                chainReflectRadius: radius);
        }
    }
}