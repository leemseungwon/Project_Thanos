using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._03._So.Reward_Data
{
    [CreateAssetMenu(fileName = "HealReward", menuName = "So/Data/Reward Data/Heal")]
    public class HealRewardSo : BaseUpgradeSo
    {
        [SerializeField, Min(0)] private int healAmount = 50;

        public int HealAmount => healAmount;

        public override bool Apply(PlayerController player)
        {
            if (player == null)
                return false;

            player.Heal(healAmount);
            return true;
        }
    }
}
