using UnityEngine;

namespace LSW._03._So.Entity_Stats.Player
{
    [CreateAssetMenu(fileName = "PlayerStatData", menuName = "So/Data/Stat Data/Player Stat Data", order = 0)]
    public class PlayerStatData : EntityStatData
    {
        public float dashSpeed = 10f;
        public float dashDuration = 0.5f;
    }
}