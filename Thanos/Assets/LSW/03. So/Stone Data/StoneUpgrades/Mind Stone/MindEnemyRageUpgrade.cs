using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Mind_Stone
{
    [CreateAssetMenu(fileName = "Mind Enemy Range Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Mind Stone Upgrade/Mind Enemy Range", order = 0)]
    public class MindEnemyRageUpgrade : BaseStoneUpgrade
    {
        [Header("Rage")]
        [SerializeField] private float duration = 5f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (stone is not MindStone mindStone)
                return;

            if (mindStone.CurrentArena == null)
                return;

            mindStone.CurrentArena.SetEnemyRage(duration);
        }
    }
}