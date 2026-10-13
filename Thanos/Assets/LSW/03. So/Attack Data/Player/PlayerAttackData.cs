using UnityEngine;

namespace LSW._03._So.Attack_Data.Player
{
    [CreateAssetMenu(fileName = "Player Attack Data", 
        menuName = "So/Data/Attack Data/Player Attack Data", order = 0)]
    public class PlayerAttackData : AttackData
    {
        public float knockbackPower = 1f;
        public float stunDuration = 1f;
    }
}