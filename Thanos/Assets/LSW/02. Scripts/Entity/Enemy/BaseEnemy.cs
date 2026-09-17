using LSW._02._Scripts.Entity.Enemy.Components;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy
{
    public class BaseEnemy : BaseEntity, IPoolable
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        public EnemyMovementComponent Movement { get; private set; }
        public EnemyCombatComponent Combat { get; private set; }       
        public bool IsFacingRight => FacingDirection > 0;
        protected int FacingDirection { get; private set; } = 1;

        public PlayerController Player => _player;
        
        private PlayerController _player;

        public virtual void Initialize()
        {
            InitStat();
            Movement.EnableMovement(true);
        }
        
        public virtual void Spawn()
        {
            InitStat();
            ResetComponents();
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

            if (_player == null)
            {
                if (SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem))
                {
                    _player = playerSystem.Player;
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
        
        public void Flip(Vector2 direction)
        {
            if (spriteRenderer == null)
                return;

            if (Mathf.Abs(direction.x) < 0.01f)
                return;

            if (direction.x > 0f)
            {
                FacingDirection = 1;
                spriteRenderer.flipX = false;
            }
            else
            {
                FacingDirection = -1;
                spriteRenderer.flipX = true;
            }
        }

        public void FlipTo(Vector2 targetPosition)
        {
            Vector2 direction =
                targetPosition - (Vector2)transform.position;

            Flip(direction);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            Movement.EnableMovement(false);
        }
        
        public virtual void Despawn() { }
    }
}