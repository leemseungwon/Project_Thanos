
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using UnityEngine;

namespace LSW._03._So.Stone_Data
{
    public abstract class BaseStone : BaseUpgradeSo
    {
        [SerializeField] private StoneType stoneType;

        public StoneType StoneType => stoneType;
        public string StoneName => Title;
        public string StoneDescription => Description;
        public Sprite StoneSprite => Icon;

        public override bool Apply(PlayerController player)
        {
            if (player == null || !player.GetCompo(out PlayerStoneComponent stones))
                return false;

            stones.GetStone(this);
            return true;
        }
        
        public abstract void Use(PlayerController owner);
        
        public virtual void OnSwitchEnter(PlayerController owner)
        {
        }
        
        public virtual void OnSwitchExit(PlayerController owner)
        {
        }
        
        public virtual void OnCombo(PlayerController owner, BaseStone previousStone)
        {
        }
    }
}
