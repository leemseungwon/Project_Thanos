using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Mind_Stone
{
    [CreateAssetMenu(fileName = "Mind Larger Area Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Mind Stone Upgrade/Mind Larger Area", order = 0)]
    public class MindLargerAreaUpgrade : BaseStoneUpgrade
    {
        [Header("Area")] [SerializeField] private float radiusMultiplier = 1.5f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            base.OnUse(player, stone);
            if (stone is not MindStone mindStone ||
                mindStone.CurrentArena == null ||
                radiusMultiplier <= 0f)
                return;

            mindStone.CurrentArena.AddRadius(
                mindStone.CurrentArena.Radius * (radiusMultiplier - 1f));
        }
    }
}