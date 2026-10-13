using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Projectiles
{
    public class BaseProjectile : MonoBehaviour, IPoolable
    {
        [SerializeField] protected float speed = 10f;
        [SerializeField] protected float lifetime = 3f;
        [SerializeField] private string poolName = "Projectile";

        protected Vector2 Direction;
        protected int Damage;
        protected AttackData AttackData;
        protected LayerMask TargetLayer;
        protected BaseEntity Owner;
        protected bool IsInitialized = false;
        private PoolSystem _poolSystem;
        private float _remainingLifetime;

        public static bool TrySpawn(string poolName, Vector3 position, Quaternion rotation,
            BaseEntity owner, Vector2 direction, int damage, AttackData attackData,
            LayerMask targetLayer, out BaseProjectile projectile)
        {
            projectile = null;
            if (SystemHandler.Instance == null ||
                !SystemHandler.Instance.GetSystem(out PoolSystem poolSystem) ||
                !poolSystem.SpawnPool(poolName, out projectile))
                return false;

            projectile.poolName = poolName;
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.Initialize(owner, direction, damage, attackData, targetLayer);
            return true;
        }

        public void Initialize()
        {
            if (SystemHandler.Instance != null)
                SystemHandler.Instance.GetSystem(out _poolSystem);
        }
        
        public void Initialize(BaseEntity owner, Vector2 direction, int damage, 
            AttackData attackData, LayerMask targetLayer)
        {
            Owner = owner;
            Direction = direction;
            Damage = damage;
            AttackData = attackData;
            TargetLayer = targetLayer;
            IsInitialized = true;
            _remainingLifetime = lifetime;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
        }
        
        public void Spawn()
        {
            Owner = null;
            Direction = Vector2.zero;
            Damage = 0;
            AttackData = null;
            TargetLayer = default;
            IsInitialized = false;
            _remainingLifetime = 0f;
        }

        protected virtual void Update()
        {
            if (!IsInitialized) 
                return;

            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                ReturnToPool();
                return;
            }

            transform.Translate(Vector2.right * (speed * Time.deltaTime));
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsInitialized) 
                return;

            if (other.TryGetComponent(out IDamageable damageable) 
                && Tools.IsSameLayerMask(other.gameObject.layer, TargetLayer))
            {
                damageable.TakeDamage(Damage, Direction, 0f, 0f, Owner);
                ReturnToPool();
            }
        }

        public void Despawn()
        {
            Owner = null;
            Direction = Vector2.zero;
            Damage = 0;
            AttackData = null;
            TargetLayer = default;
            IsInitialized = false;
            _remainingLifetime = 0f;
        }

        private void ReturnToPool()
        {
            if (_poolSystem != null && _poolSystem.HasPool(poolName))
            {
                _poolSystem.DespawnPool(poolName, this);
                return;
            }

            Despawn();
            gameObject.SetActive(false);
        }
    }
}
