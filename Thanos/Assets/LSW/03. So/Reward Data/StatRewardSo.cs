using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace LSW._03._So.Reward_Data
{
    [CreateAssetMenu(fileName = "StatReward", menuName = "So/Data/Reward Data/Stat")]
    public class StatRewardSo : BaseUpgradeSo
    {
        [SerializeField] private EntityStatType statType = EntityStatType.MaxHp;
        [FormerlySerializedAs("currencyAmount")]
        [SerializeField, Min(0)] private float statIncrease = 25f;

        public override bool Apply(PlayerController player)
        {
            if (player == null)
                return false;

            player.ChangeEntityStat(statType, MathOperation.Add, statIncrease);
            return true;
        }
    }
}
