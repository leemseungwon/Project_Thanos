
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.Components
{
    public abstract class EnemyCombatComponent : MonoBehaviour, IEntityComponent
    {
        protected BaseEntity Owner;

        public virtual void Initialize(BaseEntity owner)
        {
            Owner = owner;
        }

        public abstract void Reset();
        
        public abstract bool TryAttack(PlayerController enemy);
    }
}