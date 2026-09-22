using System;
using LSW._03._So.Animation_Datas;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies
{
    public class GolemAnimation : EntityAnimation
    {
        [field:SerializeField] public AnimationData ChaseAnimationData { get; private set; }
        [field:SerializeField] public AnimationData FrontAttackAnimationData { get; private set; }
        [field:SerializeField] public AnimationData ClapAttackAnimationData { get; private set; }
        [field:SerializeField] public AnimationData TakeDamageAnimationData { get; private set; }

        public event Action OnFrontAttackCast; 
        public event Action OnFrontAttackAnimationEnd; 
        public event Action OnClapAttackCast; 
        public event Action OnClapAttackAnimationEnd; 
        public event Action OnTakeDamageAnimationEnd;
        
        public void FrontAttackCast() => OnFrontAttackCast?.Invoke();
        public void FrontAttackAnimationEnd() => OnFrontAttackAnimationEnd?.Invoke();
        
        public void ClapAttackCast() => OnClapAttackCast?.Invoke();
        public void ClapAttackAnimationEnd() => OnClapAttackAnimationEnd?.Invoke();
        
        public void TakeDamageAnimationEnd() => OnTakeDamageAnimationEnd?.Invoke();
    }
}
