using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity
{
    public interface IDamageable
    {
        bool TakeDamage(AttackData attackData, int finalDamage, BaseEntity attacker = null);
    }
}
