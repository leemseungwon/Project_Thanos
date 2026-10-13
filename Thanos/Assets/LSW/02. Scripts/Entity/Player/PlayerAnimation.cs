
using System;
using LSW._03._So.Animation_Datas;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player
{
    public class PlayerAnimation : EntityAnimation
    {
        [Header("Animation Data List")]
        [field:SerializeField] public AnimationData LookDirectionXAnimationData { get; private set; }
        [field:SerializeField] public AnimationData LookDirectionYAnimationData { get; private set; }
        [field:SerializeField] public AnimationData MoveDirectionXAnimationData { get; private set; }
        [field:SerializeField] public AnimationData MoveDirectionYAnimationData { get; private set; }
        [field:SerializeField] public AnimationData DashAnimationData { get; private set; }
        
        [field: SerializeField] public AnimationData AttackAnimationData { get; private set; }
        
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