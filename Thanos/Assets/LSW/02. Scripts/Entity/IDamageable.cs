
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity
{
    public interface IDamageable
    {
        bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower, float stunDuration,
            BaseEntity attacker);
    }
}
