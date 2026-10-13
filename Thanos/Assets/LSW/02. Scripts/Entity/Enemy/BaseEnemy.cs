using System.Collections;
using LSW._02._Scripts.Entity.Enemy.Components;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.UI.InGame;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy
{
    public class BaseEnemy : BaseEntity, IPoolable
    {
        [SerializeField] private string damageTextPoolName = "UI_DamageText";
        
        public EnemyMovementComponent Movement { get; private set; }
        public EnemyCombatComponent Combat { get; private set; }
        public PlayerController Target => target;
        public Vector3 InitialLocalScale => _initialLocalScale;
        
        private PlayerController target;
        private Vector3 _initialLocalScale;
        private EnemyHealthBarUI _healthBar;
        private Coroutine _pullCoroutine;

        public virtual void Initialize()
        {
            InitStat();
            Movement.EnableMovement(true);
        }
        
        public virtual void Spawn()
        {
            ResetRuntimeState();
            transform.localScale = _initialLocalScale;
            InitStat();
            ResetComponents();
            _healthBar.Refresh();
            Movement.EnableMovement(true);
        }

        private void ResetComponents()
        {
            foreach (var component in Components)
            {
                component.Value.Reset();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _initialLocalScale = transform.localScale;
            _healthBar = GetComponent<EnemyHealthBarUI>();
            if (_healthBar == null)
                _healthBar = gameObject.AddComponent<EnemyHealthBarUI>();

            if (target == null)
            {
                if (SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem))
                {
                    target = playerSystem.Player;
                }
            }

            if (GetCompo(out EnemyMovementComponent movement))
            {
                Movement = movement;
            }
            
            if (GetCompo(out EnemyCombatComponent combat))
            {
                Combat = combat;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (IsDead)
                return;
        }

        public override bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower, float stunDuration,
            BaseEntity attacker)
        {
            int previousHp = CurrentHp;
            Vector3 damageTextPosition = GetDamageTextPosition();
            bool damageAccepted = base.TakeDamage(
                finalDamage, knockBackDirection, knockBackPower, stunDuration, attacker);
            int appliedDamage = previousHp - CurrentHp;

            if (damageAccepted && appliedDamage > 0 &&
                SystemHandler.Instance != null &&
                SystemHandler.Instance.GetSystem(out PoolSystem poolSystem) &&
                poolSystem.SpawnPool(damageTextPoolName, out DamageNumberUI damageText))
            {
                damageText.transform.position = damageTextPosition;
                damageText.SetDamage(appliedDamage);
            }

            return damageAccepted;
        }

        public void PullTowards(Vector2 position, float speed)
        {
            if (IsDead || Rigidbody == null || speed <= 0f || _pullCoroutine != null)
                return;

            const float pullMovementDuration = 0.9f;
            SetStun(pullMovementDuration);
            _pullCoroutine = StartCoroutine(
                PullTowardsCoroutine(position, speed, pullMovementDuration));
        }

        private IEnumerator PullTowardsCoroutine(Vector2 position, float distance, float duration)
        {
            Vector2 startPosition = Rigidbody.position;
            Vector2 targetPosition = Vector2.MoveTowards(startPosition, position, distance);
            float elapsed = 0f;

            while (elapsed < duration && !IsDead)
            {
                elapsed += Time.fixedDeltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                Rigidbody.MovePosition(Vector2.Lerp(startPosition, targetPosition, progress));
                yield return new WaitForFixedUpdate();
            }

            if (!IsDead && Rigidbody != null)
                Rigidbody.MovePosition(targetPosition);

            _pullCoroutine = null;
        }

        private Vector3 GetDamageTextPosition()
        {
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            if (spriteRenderers.Length == 0)
                return transform.position;

            float top = spriteRenderers[0].bounds.max.y;
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                    top = Mathf.Max(top, spriteRenderer.bounds.max.y);
            }

            return new Vector3(transform.position.x, top, transform.position.z);
        }

        protected override void OnDeath()
        {
            StopPulling();
            base.OnDeath();

            Movement.EnableMovement(false);
        }
        
        public virtual void Despawn()
        {
            StopPulling();
            ResetRuntimeState();
            if (Movement != null)
                Movement.EnableMovement(false);
            transform.localScale = _initialLocalScale;
        }

        private void StopPulling()
        {
            if (_pullCoroutine == null)
                return;

            StopCoroutine(_pullCoroutine);
            _pullCoroutine = null;
        }
    }
}
