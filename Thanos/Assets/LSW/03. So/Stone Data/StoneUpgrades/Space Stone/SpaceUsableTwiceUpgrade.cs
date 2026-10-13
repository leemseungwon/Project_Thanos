using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Space_Stone
{
    [CreateAssetMenu(fileName = "Space Usable Twice Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Space Stone Upgrade/Space Usable Twice", order = 0)]
    public class SpaceUsableTwiceUpgrade : BaseStoneUpgrade
    {
        public override void OnUse(PlayerController player, BaseStone stone)
        {
            // Todo
            // 두번 사용 가능
        }
    }
}