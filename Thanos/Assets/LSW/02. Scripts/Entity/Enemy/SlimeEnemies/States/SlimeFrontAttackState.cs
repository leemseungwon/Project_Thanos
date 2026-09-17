using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies.States
{
    public class SlimeFrontAttackState : EntityState
    {
        private readonly SlimeEnemy _slime;

        private Vector2 _attackDirection;

        public SlimeFrontAttackState(SlimeEnemy slime)
            : base(slime)
        {
            _slime = slime;
        }

        public override void Enter(Action endAction = null)
        {
            Debug.Log("Enter Front Attack State");
            base.Enter(endAction);
            
            Animation.SetParam(_slime.SlimeAnimation.ChaseAnimationData, false);

            _slime.Movement.EnableMovement(false);

            if (_slime.Player != null && Entity is BaseEnemy baseEnemy)
            {
                Vector2 direction =
                    _slime.Player.Rigidbody.position -
                    baseEnemy.Rigidbody.position;

                baseEnemy.Flip(direction);

                _attackDirection = direction.normalized;
            }

            Animation.SetParam(
                _slime.SlimeAnimation.FrontAttackAnimationData);
        }

        public void AttackCast()
        {
            if (_slime.Player == null)
                return;

            _slime.SlimeCombat.TryFrontAttack(
                _slime.Player,
                _attackDirection);
        }

        public void AnimationEnd()
        {
            if (_slime.StateMachine.CurrentState?.stateName !=
                SlimeEnemy.FrontAttackState)
                return;
            
            _slime.ChangeState(SlimeEnemy.ChaseState);
        }

        public override void Exit()
        {
            Debug.Log("Exit Front Attack State");
            _slime.Movement.Stop();

            base.Exit();
        }
    }
}