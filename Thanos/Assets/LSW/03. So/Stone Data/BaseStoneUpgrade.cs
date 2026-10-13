using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using UnityEngine;

namespace LSW._03._So.Stone_Data
{
    public abstract class BaseStoneUpgrade : BaseUpgradeSo
    {
        [Header("Blessing Info")]
        [SerializeField] private StoneType stoneType;

        public StoneType StoneType => stoneType;
        public string UpgradeName => Title;

        public override bool Apply(PlayerController player)
        {
            if (player == null || !player.GetCompo(out PlayerStoneComponent stones))
                return false;

            stones.AddStoneUpgrade(this);
            return true;
        }

        public virtual void OnAcquire(PlayerController player)
        {
        }
        
        public virtual void OnUse(PlayerController player, BaseStone stone)
        {
        }
        
        public virtual void OnSwitch(PlayerController player, BaseStone previousStone, 
            BaseStone currentStone)
        {
        }
    }
}
