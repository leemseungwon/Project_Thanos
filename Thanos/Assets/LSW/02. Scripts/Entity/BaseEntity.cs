using System;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Scripts.Common;
using LSW._03._So.Attack_Data;
using LSW._03._So.Entity_Stats;
using UnityEngine;

namespace LSW._02._Scripts.Entity
{
    public class BaseEntity : MonoBehaviour, IDamageable
    {
        [field:SerializeField] public EntityStatData StatData { get; private set; }
        
        protected Dictionary<Type, IEntityComponent> Components;
        public EntityStateMachine StateMachine { get; set; }
        
        public int CurrentHp { get; protected set; }
        public int MaxHp { get; set; }
        public float CurrentMoveSpeed { get; set; }
        public int CurrentAttackDamage { get; set; }

        public bool IsDead { get; protected set; }
        
        protected virtual void Awake()
        {
            Components = GetComponentsInChildren<IEntityComponent>(true)
                .ToDictionary(compo => compo.GetType());
            
            InitComponents();
            InitStat();
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
            if(StateMachine is { CurrentState: not null })
                StateMachine.Update();
        }

        protected virtual void FixedUpdate()
        {
            if(StateMachine is { CurrentState: not null })
                StateMachine.FixedUpdate();
        }

        public virtual void Heal(int amount)
        {
            if (amount <= 0)
                return;

            CurrentHp = Mathf.Min(CurrentHp + amount, MaxHp);
        }

        public virtual bool TakeDamage(AttackData attackData, int finalDamage, BaseEntity attacker = null)
        {
            if (CurrentHp <= 0)
                return false;
             
            CurrentHp -= finalDamage;
            if (CurrentHp <= 0)
            {
                OnDeath();
            }

            return true;
        }

        protected virtual void OnDeath()
        {
            IsDead = true;
        }
    }
}