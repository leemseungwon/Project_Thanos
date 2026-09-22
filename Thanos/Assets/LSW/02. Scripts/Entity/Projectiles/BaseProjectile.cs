using LSW._02._Scripts.System.PoolSystems;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Projectiles
{
    public class BaseProjectile : MonoBehaviour, IPoolable
    {
        [SerializeField] protected float speed = 10f;
        [SerializeField] protected float lifetime = 3f;

        protected Vector2 Direction;
        protected int Damage;
        protected AttackData AttackData;
        protected LayerMask TargetLayer;
        protected BaseEntity Owner;
        protected bool IsInitialized = false;

        public void Initialize() { }
        
        public void Initialize(BaseEntity owner, Vector2 direction, int damage, 
            AttackData attackData, LayerMask targetLayer)
        {
            Owner = owner;
            Direction = direction;
            Damage = damage;
            AttackData = attackData;
            TargetLayer = targetLayer;
            IsInitialized = true;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            Destroy(gameObject, lifetime);
        }
        
        public void Spawn()
        {
            Owner = null;
            Direction = Vector2.zero;
            Damage = 0;
            AttackData = null;
            IsInitialized = false;
        }

        protected virtual void Update()
        {
            if (!IsInitialized) 
                return;

            transform.Translate(Vector2.right * (speed * Time.deltaTime));
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsInitialized) 
                return;

            if (other.TryGetComponent(out IDamageable damageable) 
                && Tools.IsSameLayerMask(other.gameObject.layer, TargetLayer))
            {
                damageable.TakeDamage(Damage, Direction, AttackData.knockbackPower, 0f, Owner, AttackData);
                Destroy(gameObject);
            }
        }

        public void Despawn()
        {
            
        }
    }
}