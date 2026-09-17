
using UnityEngine;

namespace LSW._02._Scripts.Entity
{
    public interface IDamageable
    {
        bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower = 0f,
            float stunDuration = 0f, BaseEntity attacker = null);

    }
}
