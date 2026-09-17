using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Scripts.Common;
using LSW._03._So.Entity_Stats;
using UnityEngine;

namespace LSW._02._Scripts.Entity
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseEntity : MonoBehaviour, IDamageable
    {
        [field:SerializeField] public EntityStatData StatData { get; private set; }
        public EntityAnimation Animation { get; private set; }
        
        protected Dictionary<Type, IEntityComponent> Components;
        
        public EntityStateMachine StateMachine { get; set; }

        public Rigidbody2D Rigidbody => _rigidbody;
        
        public int CurrentHp { get; protected set; }
        public int MaxHp { get; set; }
        public float CurrentMoveSpeed { get; set; }
        public int CurrentAttackDamage { get; set; }
        
        public bool IsStunned { get; private set; }
        public bool IsDead { get; protected set; }
        
        private Rigidbody2D _rigidbody;
        private Coroutine _knockbackCoroutine;
        private Coroutine _stunCoroutine;
        private const float KnockbackDuration = 0.2f;
        
        public event Action<int, int> OnHealthChanged;
        public event Action OnDeathEvent;
        
        protected virtual void Awake()
        {
            Components = GetComponentsInChildren<IEntityComponent>(true)
                .ToDictionary(compo => compo.GetType());
            
            InitComponents();
            InitStat();

            if (GetCompo(out EntityAnimation anima))
            {
                Animation = anima;
            }

            _rigidbody = GetComponent<Rigidbody2D>();
        }
        
        protected virtual void InitComponents()
        {
            Components.Values.ToList().ForEach(component => component.Initialize(this));
        }
        
        public void InitStat()
        {
            ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Set, StatData.initMaxHp);
            ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Set, StatData.initAttackDamage);
            ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Set, StatData.initMoveSpeed);

            CurrentHp = MaxHp;
            IsDead = false;
        }
        
        public bool GetCompo<T>(out T component) where T : IEntityComponent
        {
            component = default;
            IEntityComponent findComponent = Components.Values.FirstOrDefault(c => c is T);
            if (findComponent is T findCompo)
            {
                component = findCompo;
                return true;
            }
            return false;
        }

        public void ChangeEntityStat(EntityStatType type, MathOperation operation, float value)
        {
            switch (type)
            {
                case EntityStatType.MoveSpeed:
                    CurrentMoveSpeed = operation switch
                    {
                        MathOperation.Add => CurrentMoveSpeed + value,
                        MathOperation.Subtract => CurrentMoveSpeed - value,
                        MathOperation.Multiply => CurrentMoveSpeed * value,
                        MathOperation.Divide => CurrentMoveSpeed / value,
                        MathOperation.Modulo => CurrentMoveSpeed % value,
                        MathOperation.Set => value,
                        _ => CurrentMoveSpeed
                    };
                    break;
                case EntityStatType.MaxHp:
                    MaxHp = operation switch
                    {
                        MathOperation.Add => (int)(MaxHp + value),
                        MathOperation.Subtract => (int)(MaxHp - value),
                        MathOperation.Multiply => (int)(MaxHp * value),
                        MathOperation.Divide => (int)(MaxHp / value),
                        MathOperation.Modulo => (int)(MaxHp % value),
                        MathOperation.Set => (int)value,
                        _ => MaxHp
                    };
                    break;
                case EntityStatType.AttackDamage:
                    CurrentAttackDamage = operation switch
                    {
                        MathOperation.Add => (int)(CurrentAttackDamage + value),
                        MathOperation.Subtract => (int)(CurrentAttackDamage - value),
                        MathOperation.Multiply => (int)(CurrentAttackDamage * value),
                        MathOperation.Divide => (int)(CurrentAttackDamage / value),
                        MathOperation.Modulo => (int)(CurrentAttackDamage % value),
                        MathOperation.Set => (int)value,
                        _ => CurrentAttackDamage
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        protected virtual void Update()
        {
            if (StateMachine is { CurrentState: not null })
                StateMachine.Update();
        }

        protected virtual void FixedUpdate()
        {
            if (StateMachine is { CurrentState: not null })
                StateMachine.FixedUpdate();
        }

        public virtual void Heal(int amount)
        {
            if (amount <= 0)
                return;

            CurrentHp = Mathf.Min(CurrentHp + amount, MaxHp);
        }
        
        public virtual bool TakeDamage(int finalDamage, Vector2 knockBackDirection, float knockBackPower = 0f, float stunDuration = 0f, BaseEntity attacker = null)
        {
            if (CurrentHp <= 0)
                return false;
             
            CurrentHp -= finalDamage;
            
            SetStun(stunDuration);
            Knockback(knockBackPower, knockBackDirection);
            
            if (CurrentHp <= 0)
            {
                OnDeath();
            }
            
            OnHealthChanged?.Invoke(CurrentHp, MaxHp);
            return true;
        }

        public virtual void Knockback(float knockBackPower, Vector2 direction)
        {
            if (Rigidbody == null || knockBackPower <= 0f)
                return;

            direction.Normalize();

            if (_knockbackCoroutine != null)
                StopCoroutine(_knockbackCoroutine);

            _knockbackCoroutine = StartCoroutine(KnockbackCoroutine(direction, knockBackPower));
        }

        private IEnumerator KnockbackCoroutine(Vector2 direction, float knockbackPower)
        {
            direction = direction.normalized;

            Vector2 startPosition = Rigidbody.position;

            Vector2 targetPosition =
                startPosition + direction * knockbackPower;

            float elapsed = 0f;

            while (elapsed < KnockbackDuration)
            {
                elapsed += Time.fixedDeltaTime;

                float t = Mathf.Clamp01(elapsed / KnockbackDuration);

                Rigidbody.MovePosition(
                    Vector2.Lerp(startPosition, targetPosition, t)
                );

                yield return new WaitForFixedUpdate();
            }

            Rigidbody.MovePosition(targetPosition);
            Rigidbody.linearVelocity = Vector2.zero;
            
            _knockbackCoroutine = null;
        }
        
        public void SetStun(float duration)
        {
            if(_stunCoroutine != null)
                StopCoroutine(_stunCoroutine);
            
            _stunCoroutine = StartCoroutine(StunCoroutine(duration));
        }

        private IEnumerator StunCoroutine(float duration)
        {
            IsStunned = true;

            yield return new WaitForSeconds(duration);
            
            IsStunned = false;
            _stunCoroutine = null;
        }
        
        protected virtual void OnDeath()
        {
            IsDead = true;
            OnDeathEvent?.Invoke();
        }

        public virtual void OnDestroy()
        {
            if (StateMachine != null)
            {
                StateMachine.OnDestroy();
            }
            
            if(_stunCoroutine != null)
                StopCoroutine(_stunCoroutine);
            
            if(_knockbackCoroutine != null)
                StopCoroutine(_knockbackCoroutine);
        }
    }
}