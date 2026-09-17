using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies.States
{
    public class SlimeChaseState : EntityState
    {
        private readonly SlimeEnemy _slime;

        public SlimeChaseState(SlimeEnemy slime)
            : base(slime)
        {
            _slime = slime;
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            Debug.Log("Enter Chase State");
            
            Animation.SetParam(_slime.SlimeAnimation.ChaseAnimationData, true);
            _slime.Movement.EnableMovement(true);
        }

        public override void Update()
        {
            if (Entity.IsDead)
                return;

            if (_slime.Player == null)
                return;

            Vector2 playerPosition = _slime.Player.Rigidbody.position;
            Vector2 direction = playerPosition - Entity.Rigidbody.position;

            if (Entity is BaseEnemy baseEnemy)
            {
                baseEnemy.Flip(direction);
            }

            if (_slime.SlimeCombat.CanHitWithFrontAttack(direction))
            {
                if (_slime.TryChooseAttack())
                {
                    _slime.Movement.Stop();
                    return;
                }
            }

            _slime.Movement.MoveTo(playerPosition);
        }

        public override void Exit()
        {
            Debug.Log("Exit Chase State");
            _slime.Movement.Stop();

            base.Exit();
        }
    }
}