using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "Time Stone", menuName = "So/Data/Stone Data/Stones/Time Stone", order = 0)]
    public class TimeStone : BaseStone
    {
        [Header("Time")]
        [SerializeField] private int healAmount = 15;
        
        public override void Use(PlayerController player)
        {
            if (player == null)
                return;

            player.Heal(healAmount);
        }
    }
}