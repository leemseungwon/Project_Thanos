using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies.States
{
    public class GolemFrontAttackState : EntityState
    {
        private readonly GolemEnemy _golem;

        public GolemFrontAttackState(GolemEnemy golem)
            : base(golem)
        {
            _golem = golem;
        }

        public override void Enter(Action endAction = null)
        {
            Debug.Log("Enter Front Attack State");
            base.Enter(endAction);
            
            Animation.SetParam(_golem.GolemAnimation.ChaseAnimationData, false);

            _golem.Movement.EnableMovement(false);

            if (_golem.Player != null && Entity is BaseEnemy baseEnemy)
            {
                Vector2 direction =
                    _golem.Player.Rigidbody.position -
                    baseEnemy.Rigidbody.position;

                baseEnemy.Flip(direction);
            }

            Animation.SetParam(
                _golem.GolemAnimation.FrontAttackAnimationData);
        }

        public override void Update()
        {
            base.Update();
            
            if (StateTimer > 2.0f)
            {
                Debug.LogWarning("GolemFrontAttackState timeout! Forcing transition to ChaseState.");
                AnimationEnd();
            }
        }

        public void AttackCast()
        {
            _golem.GolemCombat.ApplyFrontAttackCooldown();
            _golem.GolemCombat.PerformFrontAttack();
        }

        public void AnimationEnd()
        {
            if (_golem.StateMachine.CurrentState?.stateName !=
                GolemEnemy.FrontAttackState)
                return;
            
            _golem.ChangeState(GolemEnemy.ChaseState);
        }

        public override void Exit()
        {
            Debug.Log("Exit Front Attack State");
            _golem.Movement.Stop();

            base.Exit();
        }
    }
}
