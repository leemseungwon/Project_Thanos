using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies.States
{
    public class GolemTakeDamageState : EntityState
    {
        private readonly GolemEnemy _golem;

        public GolemTakeDamageState(GolemEnemy golem)
            : base(golem)
        {
            _golem = golem;
        }

        public override void Enter(Action endAction = null)
        {
            Debug.Log("Enter Take Damage State");
            base.Enter(endAction);

            _golem.Movement.EnableMovement(false);
            _golem.Movement.Stop();

            Animation.SetParam(_golem.GolemAnimation.TakeDamageAnimationData);
        }

        public override void Update()
        {
            base.Update();
            
            if (StateTimer > 1.5f)
            {
                Debug.LogWarning("GolemTakeDamageState timeout! Forcing transition to ChaseState.");
                AnimationEnd();
            }
        }

        public void AnimationEnd()
        {
            if (_golem.StateMachine.CurrentState?.stateName != GolemEnemy.TakeDamageState)
                return;

            _golem.ChangeState(GolemEnemy.ChaseState);
        }

        public override void Exit()
        {
            Debug.Log("Exit Take Damage State");
            base.Exit();
        }
    }
}
