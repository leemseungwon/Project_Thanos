
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data
{
    public abstract class BaseStone : ScriptableObject
    {
        [Header("Stone Info")]
        [SerializeField] private string stoneName;

        public string StoneName => stoneName;
        
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