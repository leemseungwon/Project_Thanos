using System;
using LSW._03._So.Animation_Datas;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies
{
    public class SlimeAnimation : EntityAnimation
    {
        [field:SerializeField] public AnimationData ChaseAnimationData { get; private set; }
        [field:SerializeField] public AnimationData WhirlwindAnimationData { get; private set; }
        [field:SerializeField] public AnimationData FrontAttackAnimationData { get; private set; }
        [field:SerializeField] public AnimationData TakeDamageAnimationData { get; private set; }
        
        public event Action OnWhirlwindAttackCast; 
        public event Action OnWhirlwindAnimationEnd; 
        public event Action OnFrontAttackCast; 
        public event Action OnFrontAttackAnimationEnd; 
        
        public void WhirlwindAttackCast()
        {
            OnWhirlwindAttackCast?.Invoke();
        }

        public void WhirlwindAnimationEnd()
        {
            OnWhirlwindAnimationEnd?.Invoke();
        }
        
        public void FrontAttackCast()
        {
            OnFrontAttackCast?.Invoke();
        }

        public void FrontAttackAnimationEnd()
        {
            OnFrontAttackAnimationEnd?.Invoke();
        }
    }
}