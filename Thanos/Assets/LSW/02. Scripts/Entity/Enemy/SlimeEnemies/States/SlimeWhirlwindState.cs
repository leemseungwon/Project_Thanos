using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies.States
{
    public class SlimeWhirlwindState : EntityState
    {
        private readonly SlimeEnemy _slime;

        public SlimeWhirlwindState(SlimeEnemy slime)
            : base(slime)
        {
            _slime = slime;
        }

        public override void Enter(Action endAction = null)
        {
            Debug.Log("Enter Whirlwind State");
            base.Enter(endAction);

            Animation.SetParam(_slime.SlimeAnimation.ChaseAnimationData, false);
            _slime.Movement.EnableMovement(false);

            _slime.SetWhirlwindHitbox(false);

            Animation.SetParam(
                _slime.SlimeAnimation.WhirlwindAnimationData);
        }

        // Animation Event
        public void AttackCast()
        {
            _slime.SetWhirlwindHitbox(true);
        }

        // Animation Event
        public void AnimationEnd()
        {
            _slime.SetWhirlwindHitbox(false);

            _slime.ChangeState(
                SlimeEnemy.ChaseState);
        }

        public override void Exit()
        {
            Debug.Log("Exit Whirlwind State");
            
            _slime.SetWhirlwindHitbox(false);

            _slime.Movement.Stop();

            base.Exit();
        }
    }
}