using UnityEngine;

namespace LSW._03._So.Attack_Data
{
    [CreateAssetMenu(fileName = "Attack Data", menuName = "So/Data/Attack Data", order = 0)]
    public class AttackData : ScriptableObject
    {
        public int attackDamage = 10;
        public float knockbackPower = 1f;
        public float stunDuration = 1f;
    }
}