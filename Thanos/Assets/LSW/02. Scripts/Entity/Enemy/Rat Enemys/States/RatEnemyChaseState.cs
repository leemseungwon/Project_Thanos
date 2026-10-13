using System;

namespace LSW._02._Scripts.Entity.Enemy.Rat_Enemys.States
{
    public class RatEnemyChaseState : EntityState
    {
        private readonly RatEnemy ratEnemy;
        
        public RatEnemyChaseState(RatEnemy enemy) : base(enemy)
        {
            ratEnemy = enemy;
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
        }

        public override void Update()
        {
            base.Update();
            
            if (ratEnemy.Target == null || ratEnemy.IsStunned)
            {
                return;
            }

            if (ratEnemy.Target.IsDead)
            {
                ratEnemy.Movement.Stop();
                return;
            }

            if (ratEnemy.Movement.IsPlayerInStopRange)
            {
                ratEnemy.Movement.Stop();
                ratEnemy.StateMachine.ChangeState(RatEnemy.AttackState);
                ratEnemy.RatAnimation.SetParam(ratEnemy.RatAnimation.IsMoveAnimationData, false);

                return;
            }

            ratEnemy.RatAnimation.SetParam(ratEnemy.RatAnimation.IsMoveAnimationData, true);
            ratEnemy.Movement.MoveTo(ratEnemy.Target.transform.position);
        }

        public override void Exit()
        {
            base.Exit();
            ratEnemy.Movement.Stop();
        }
    }
}