using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Mind_Stone
{
    [CreateAssetMenu(fileName = "Mind Damage Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Mind Stone Upgrade/Mind Damage", order = 0)]
    public class MindDamageUpgrade : BaseStoneUpgrade
    {
        [Header("Damage")]
        [SerializeField] private int damagePerTick = 3;
        [SerializeField] private float tickInterval = 1f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null || stone is not MindStone mindStone)
                return;

            if (mindStone.CurrentArena == null)
                return;

            mindStone.CurrentArena.SetDamageOverTime(
                damagePerTick + player.CurrentAttackDamage, tickInterval);
        }
    }
}