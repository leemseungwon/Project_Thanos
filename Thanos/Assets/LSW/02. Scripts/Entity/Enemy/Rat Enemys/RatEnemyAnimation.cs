
using System;
using LSW._03._So.Animation_Datas;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.Rat_Enemys
{
    public class RatEnemyAnimation : EntityAnimation
    {
        [field:SerializeField] public AnimationData LookDirectionXAnimationData { get; private set; }
        [field:SerializeField] public AnimationData LookDirectionYAnimationData { get; private set; }
        [field: SerializeField] public AnimationData AttackAnimationData { get; private set; }
        [field: SerializeField] public AnimationData GetHitAnimationData { get; private set; }
        [field: SerializeField] public AnimationData GetHitDirXAnimationData { get; private set; }
        [field: SerializeField] public AnimationData GetHitDirYAnimationData { get; private set; }
        
        public event Action OnAttackCast;
        public event Action OnAttackAnimationEnd;
        
        public void AttackCast()
        {    
            OnAttackCast?.Invoke();
        }

        public void AttackAnimationEnd()
        {
            OnAttackAnimationEnd?.Invoke();
        }
    }
}