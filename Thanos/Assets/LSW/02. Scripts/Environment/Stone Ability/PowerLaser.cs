using System.Collections.Generic;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.Environment.Stone_Ability
{
    public class PowerLaser : MonoBehaviour, IPoolable
    {
        public string powerLaserPoolName = "StoneAbility_PowerLaser";
        
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float width = 0.2f;

        private PlayerController _owner;
        private PoolSystem _poolSystem;

        private int _damage = 10;
        private float _knockbackPower = 0f;
        private float _range = 5f;
        private LayerMask _enemyLayer;
        
        private float _timer;

        private readonly HashSet<BaseEnemy> _damagedEnemies = new();

        public void Initialize()
        {
            SystemHandler.Instance.GetSystem(out _poolSystem);
        }

        public void Setup(PlayerController owner, Vector2 direction,
            float range, int damage, float knockbackPower, LayerMask enemyLayer)
        {
            _owner = owner;
            _range = range;
            _damage = damage;
            _knockbackPower = knockbackPower; 
            _enemyLayer = enemyLayer;

            transform.position = owner.transform.position;
            SetDirection(direction);

            _damagedEnemies.Clear();
            ApplyLaserDamage();
        }
        
        public void Spawn()
        {
            _timer = 0f;
            _damagedEnemies.Clear();
        }

        public void Despawn()
        {
            _owner = null;
            _timer = 0f;

            _damagedEnemies.Clear();
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= duration)
            {
                DespawnLaser();
            }
        }

        private void SetDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
                return;

            direction.Normalize();

            float angle = Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(0f, 0f, angle);

            /*
             * Laser 프리팹의 기본 방향을
             * 오른쪽(Vector2.right)으로 만든다는 전제.
             *
             * Sprite / LineRenderer가 왼쪽 기준이라면
             * 여기의 회전을 +180도 조정하면 됨.
             */
        }

        private void ApplyLaserDamage()
        {
            if (_owner == null)
                return;

            Vector2 origin = transform.position;

            Vector2 direction = transform.right;

            RaycastHit2D[] hits = Physics2D.RaycastAll(
               origin, direction, _range, _enemyLayer);

            foreach (RaycastHit2D hit in hits)
            {
                BaseEnemy enemy = hit.collider.GetComponentInParent<BaseEnemy>();

                if (enemy == null)
                    continue;

                if (enemy.IsDead)
                    continue;

                if (!_damagedEnemies.Add(enemy))
                    continue;

                Vector2 knockbackDirection = (enemy.transform.position -
                                              _owner.transform.position).normalized;
                int finalDamage = _damage + _owner.CurrentAttackDamage;

                enemy.TakeDamage(finalDamage, knockbackDirection, _knockbackPower,
                    0f, _owner);
            }
        }

        private void DespawnLaser()
        {
            if (_poolSystem != null)
            {
                _poolSystem.DespawnPool(powerLaserPoolName, this);
                return;
            }
            Despawn();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawRay(transform.position,
                transform.right * _range);
        }
    }
}