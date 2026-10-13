using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._03._So.Reward_Data
{
    [CreateAssetMenu(fileName = "UpgradeChoiceReward", menuName = "So/Data/Reward Data/Choose Stone Upgrade")]
    public class UpgradeChoiceRewardSo : BaseUpgradeSo
    {
        public override bool Apply(PlayerController player)
        {
            return player != null;
        }
    }
}
