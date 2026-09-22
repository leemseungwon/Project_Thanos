using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.GolemEnemies.States
{
    public class GolemChaseState : EntityState
    {
        private readonly GolemEnemy _golem;

        public GolemChaseState(GolemEnemy golem)
            : base(golem)
        {
            _golem = golem;
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            Debug.Log("Enter Chase State");
            
            Animation.SetParam(_golem.GolemAnimation.ChaseAnimationData, true);
            _golem.Movement.EnableMovement(true);
        }

        public override void Update()
        {
            if (Entity.IsDead || _golem.IsStunned || _golem.Player == null)
            {
                _golem.Movement?.Stop();
                return;
            }

            Vector2 playerPosition = _golem.Player.Rigidbody.position;
            Vector2 myPosition = _golem.transform.position;
            Vector2 direction = (myPosition.x - playerPosition.x) > 0 ? Vector2.left : Vector2.right;

            if (Entity is BaseEnemy baseEnemy)
            {
                baseEnemy.Flip(direction);
            }

            // 거리 체크 및 공격 시도
            float distance = Vector2.Distance(myPosition, playerPosition);
            
            if (distance <= 4f)
            {
                if (_golem.TryChooseAttack())
                {
                    _golem.Movement?.Stop();
                    return;
                }
            }

            if (_golem.Movement != null)
            {
                bool canMove = _golem.Movement.MoveTo(playerPosition);
            
                if (!canMove)
                    _golem.Movement.Stop();
                
                _golem.Movement.EnableMovement(canMove);
                _golem.GolemAnimation.SetParam(_golem.GolemAnimation.ChaseAnimationData, canMove);
            }
        }

        public override void Exit()
        {
            Debug.Log("Exit Chase State");
            _golem.Movement.Stop();

            base.Exit();
        }
    }
}
