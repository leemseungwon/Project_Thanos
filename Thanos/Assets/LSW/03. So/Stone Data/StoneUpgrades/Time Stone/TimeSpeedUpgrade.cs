using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Time_Stone
{
    [CreateAssetMenu(fileName = "Time Speed Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Time Stone Upgrade/Time Speed", order = 0)]
    public class TimeSpeedUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float duration = 3f;
        [SerializeField] private float moveSpeedMultiplier = 1.5f;
        [SerializeField] private float attackCooldownMultiplier = 0.5f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player.GetCompo(out EntityBuffComponent buffCompo))
            {
                buffCompo.ApplyAttackCooldownMultiplier("Time Speed", attackCooldownMultiplier, duration);
                buffCompo.ApplyMoveSpeedMultiplier("Time Speed", moveSpeedMultiplier, duration);
            }
        }
    }
}