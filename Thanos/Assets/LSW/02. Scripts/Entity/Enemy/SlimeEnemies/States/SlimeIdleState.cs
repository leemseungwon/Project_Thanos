using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies.States
{
    public class SlimeIdleState : EntityState
    {
        private readonly SlimeEnemy _slime;

        private float _timer;

        private const float IdleTime = 0.5f;

        public SlimeIdleState(SlimeEnemy slime)
            : base(slime)
        {
            _slime = slime;
        }

        public override void Enter(Action endAction = null)
        {
            Debug.Log("Enter Idle State");
            base.Enter(endAction);

            _timer = IdleTime;

            _slime.Movement.EnableMovement(false);
            Animation.SetParam(_slime.SlimeAnimation.ChaseAnimationData, false);
        }

        public override void Update()
        {
            if (Entity.IsDead)
                return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                _slime.ChangeState(SlimeEnemy.ChaseState);
            }
        }

        public override void Exit()
        {
            Debug.Log("Exit Idle State");
            _slime.Movement.Stop();

            base.Exit();
        }
    }
}