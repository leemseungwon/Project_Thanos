using System;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies.States
{
    public class GolemClapState : EntityState
    {
        private readonly GolemEnemy _golem;

        public GolemClapState(GolemEnemy entity) : base(entity)
        {
            _golem = entity;
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            _golem.Movement?.Stop();
            _golem.Animation.SetParam(_golem.GolemAnimation.ClapAttackAnimationData);
        }

        public override void Update()
        {
            base.Update();
            if (StateTimer > 2.5f)
            {
                AnimationEnd();
            }
        }

        public void AttackCast()
        {
            _golem.GolemCombat.ApplyClapCooldown();
            _golem.GolemCombat.PerformClapAttack();
        }

        public void AnimationEnd()
        {
            if (_golem.StateMachine.CurrentState?.State != this)
                return;

            _golem.ChangeState(GolemEnemy.ChaseState);
        }
    }
}
