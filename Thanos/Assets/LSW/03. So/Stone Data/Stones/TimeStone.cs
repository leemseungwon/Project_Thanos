using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "Time Stone", menuName = "So/Data/Stone Data/Time Stone", order = 0)]
    public class TimeStone : BaseStone
    {
        [Header("Time")]
        [SerializeField] private int healAmount = 15;
        [SerializeField] private float cooldown = 10f;

        public float Cooldown => cooldown;
        
        private PlayerStoneComponent _playerStoneComponent;
        
        public override void Use(PlayerController player)
        {
            if(_playerStoneComponent != null || player.GetCompo(out _playerStoneComponent))
            { 
                player.Heal(healAmount);    
                _playerStoneComponent.ResetTimeStoneCooldown(this);
            }
        }
    }
}