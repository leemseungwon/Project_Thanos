
using System;
using LSW._03._So.Animation_Datas;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player
{
    public class PlayerAnimation : EntityAnimation
    {
        [Header("Animation Data List")]
        [field:SerializeField] public AnimationData MoveAnimationData { get; private set; }
        [field:SerializeField] public AnimationData DashAnimationData { get; private set; }
        
        [field: SerializeField] public AnimationData Attack1AnimationData { get; private set; }
        [field: SerializeField] public AnimationData Attack2AnimationData { get; private set; }
        [field: SerializeField] public AnimationData Attack3AnimationData { get; private set; }
        [field: SerializeField] public AnimationData Attack4AnimationData { get; private set; }
        
        [field:SerializeField] public AnimationData ParryAnimationData { get; private set; }
        [field:SerializeField] public AnimationData IsParrySuccessAnimationData { get; private set; }
        [field:SerializeField] public AnimationData IsParryFailAnimationData { get; private set; }
        
        public event Action OnAttackCast;
        public event Action OnAttackAnimationEnd;
        
        public event Action OnParryAttackCast;
        public event Action OnReadyParryAnimationEnd;
        public event Action OnParryAnimationEnd;
        
        public void AttackCast()
        {    
            OnAttackCast?.Invoke();
        }

        public void AttackAnimationEnd()
        {
            OnAttackAnimationEnd?.Invoke();
        }

        public void ParryAttackCast()
        {
            OnParryAttackCast?.Invoke();
        }
        
        public void ReadyParryAnimationEnd()
        {
            OnReadyParryAnimationEnd?.Invoke();
        }       
        
        public void ParryAnimationEnd()
        {
            OnParryAnimationEnd?.Invoke();
        }
    }
}