using System;
using LSW._02._Scripts.Entity.Enemy.Rat_Enemys.Components;

namespace LSW._02._Scripts.Entity.Enemy.Rat_Enemys.States
{
    public class RatEnemyAttackState : EntityState
    {
        private readonly RatEnemy _ratEnemy;
        private readonly RatEnemyCombatComponent _combat;
        
        public RatEnemyAttackState(RatEnemy enemy) : base(enemy)
        {
            _ratEnemy = enemy;
            if (_ratEnemy != null)
            {
                _combat = _ratEnemy.Combat as RatEnemyCombatComponent;
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);

            _ratEnemy.IsAttacking = true;
            if (!_combat.TryAttack(_ratEnemy.Target))
            {
                ChangeToNextState();
                return;
            }
            _combat.OnAttackFinished += ChangeToNextState;
        }

        private void ChangeToNextState()
        {
            _ratEnemy.StateMachine.ChangeState(RatEnemy.ChaseState);
        }
        
        public override void Exit()
        {
            _combat.OnAttackFinished -= ChangeToNextState;
            _ratEnemy.IsAttacking = false;
            base.Exit();
        }
    }
}