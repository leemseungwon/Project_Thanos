using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data
{
    public abstract class BaseStoneUpgrade : ScriptableObject
    {
        [Header("Blessing Info")]
        [SerializeField] private StoneType stoneType;
        [SerializeField] private string blessingName;

        [TextArea] 
        [SerializeField] private string description;

        public StoneType StoneType => stoneType;
        public string BlessingName => blessingName;
        public string Description => description;

        public virtual void OnUse(PlayerController player, BaseStone stone)
        {
        }
        
        public virtual void OnSwitch(PlayerController player, BaseStone previousStone, 
            BaseStone currentStone)
        {
        }
    }
}