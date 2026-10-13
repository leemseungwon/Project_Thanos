using UnityEngine;

namespace LSW._03._So.Attack_Data.Enemy
{
    [CreateAssetMenu(fileName = "Enemy Attack Data",
        menuName = "So/Data/Attack Data/Enemy Attack Data", order = 0)]
    public class EnemyAttackData : AttackData
    {
        public float attackRadius;
    }
}