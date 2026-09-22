using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies.States
{
    public class GolemIdleState : EntityState
    {
        private readonly GolemEnemy _golem;

        private float _timer;

        private const float IdleTime = 0.5f;

        public GolemIdleState(GolemEnemy golem)
            : base(golem)
        {
            _golem = golem;
        }

        public override void Enter(Action endAction = null)
        {
            Debug.Log("Enter Idle State");
            base.Enter(endAction);

            _timer = IdleTime;

            _golem.Movement.EnableMovement(false);
            Animation.SetParam(_golem.GolemAnimation.ChaseAnimationData, false);
        }

        public override void Update()
        {
            if (Entity.IsDead)
                return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f && !_golem.Movement.IsPlayerInStopRange)
            {
                _golem.ChangeState(GolemEnemy.ChaseState);
            }
        }

        public override void Exit()
        {
            Debug.Log("Exit Idle State");
            _golem.Movement.Stop();

            base.Exit();
        }
    }
}
