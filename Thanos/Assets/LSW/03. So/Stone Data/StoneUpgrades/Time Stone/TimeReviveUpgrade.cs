using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Time_Stone
{
    [CreateAssetMenu(fileName = "Time Revive Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Time Stone Upgrade/Time Revive", order = 0)]
    public class TimeReviveUpgrade : BaseStoneUpgrade
    {
        public override void OnAcquire(PlayerController player)
        {
            if (player == null ||
                !player.GetCompo(out PlayerStoneComponent stoneComponent))
                return;
            
            stoneComponent.EnableTimeRevive();
        }
    }
}