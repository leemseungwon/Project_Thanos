using System.Collections;
using System.Collections.Generic;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.Environment.Stone_Ability
{
    public class MindStoneArea : MonoBehaviour, IPoolable
    {
        [SerializeField] private string mindAreaPoolName = "StoneAbility_StoneArena";
        [SerializeField] private float tickInterval = 0.2f;
        [SerializeField] private float disableDuration = 3f;
        [SerializeField] private LayerMask enemyLayer;

        public float Radius => _radius;
        
        private PoolSystem _poolSystem;
        private BaseEntity _owner;
        private Coroutine _arenaCoroutine;
        
        private float _radius;
        private float _duration;

        private bool _hasDamage;
        private int _damagePerTick;
        private float _damageTickInterval;

        private bool _hasEnemyRage;
        private float _enemyRageDuration;
        
        private readonly HashSet<BaseEntity> _affectedEnemies = new();
        private readonly HashSet<BaseEntity> _damageAppliedEnemies = new();

        public void Initialize()
        {
            if (SystemHandler.Instance != null)
                SystemHandler.Instance.GetSystem(out _poolSystem);
        }

        public void SetInitialize(BaseEntity owner, Vector2 position, float radius,
            float duration)
        {
            _owner = owner;
            transform.position = position;
            _radius = radius;
            _duration = duration > 0f ? duration : disableDuration;
            UpdateVisualScale();

            if (_arenaCoroutine != null)
                StopCoroutine(_arenaCoroutine);
            _arenaCoroutine = StartCoroutine(ArenaCoroutine());
        }

        public void SetDamageOverTime(int damagePerTick, 
            float tickIntervals)
        {
            if (damagePerTick <= 0)
                return;

            if (tickIntervals <= 0f)
                return;

            _hasDamage = true;
            _damagePerTick = damagePerTick;
            _damageTickInterval = tickIntervals;
        }

        public void SetEnemyRage(float rageDuration)
        {
            if (rageDuration <= 0f)
                return;

            _hasEnemyRage = true;
            _enemyRageDuration = rageDuration;
        }

        public void AddRadius(float radius)
        {
            _radius += radius;
            UpdateVisualScale();
        }

        public void Spawn()
        {
            _affectedEnemies.Clear();
            _damageAppliedEnemies.Clear();

            if (_arenaCoroutine != null)
            {
                StopCoroutine(_arenaCoroutine);
                _arenaCoroutine = null;
            }
        }

        public void Despawn()
        {
            if (_arenaCoroutine != null)
            {
                StopCoroutine(_arenaCoroutine);
                _arenaCoroutine = null;
            }

            _affectedEnemies.Clear();
            _damageAppliedEnemies.Clear();
            _owner = null;
            _duration = 0f;
            ResetArenaData();
        }

        private IEnumerator ArenaCoroutine()
        {
            yield return null;
            float endTime = Time.time + _duration;

            while (Time.time < endTime)
            {
                float remainingDuration = endTime - Time.time;
                ApplyArenaEffect(remainingDuration);
                yield return new WaitForSeconds(
                    Mathf.Min(tickInterval, remainingDuration));
            }

            _arenaCoroutine = null;
            ApplyEnemyRage();
            _poolSystem.DespawnPool(mindAreaPoolName, this);
        }

        private void ApplyArenaEffect(float remainingDuration)
        {
            if (_owner == null || _owner.IsDead)
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,
                _radius, enemyLayer);

            HashSet<BaseEntity> currentEnemies = new();

            foreach (Collider2D hit in hits)
            {
                BaseEntity entity = hit.GetComponentInParent<BaseEntity>();

                if (entity == null || entity == _owner ||
                    entity.IsDead || !currentEnemies.Add(entity))
                {
                    continue;
                }

                if (_hasEnemyRage)
                {
                    _affectedEnemies.Add(entity);
                }

                if (!entity.GetCompo(out EntityBuffComponent buff))
                {
                    continue;
                }

                buff.ApplyActionDisable(remainingDuration);

                if (_hasDamage && _damageAppliedEnemies.Add(entity))
                {
                    buff.ApplyDamageOverTime("MindStone_Damage", _owner, _damagePerTick, 
                        _damageTickInterval, remainingDuration);
                }
            }
        }

        private void UpdateVisualScale()
        {
            transform.localScale = new Vector3(_radius * 2f, _radius * 2f, 1f);
        }

        private void ApplyEnemyRage()
        {
            if (!_hasEnemyRage)
                return;

            foreach (BaseEntity entity in _affectedEnemies)
            {
                if (entity == null || entity.IsDead)
                    continue;

                if (!entity.GetCompo(out EntityBuffComponent buff))
                {
                    continue;
                }

                buff.ApplyConfusionAfterDelay(0f, _enemyRageDuration, _radius,
                    1f, enemyLayer);
            }
        }

        private void ResetArenaData()
        {
            _hasDamage = false;
            _damagePerTick = 0;
            _damageTickInterval = 0f;

            _hasEnemyRage = false;
            _enemyRageDuration = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
