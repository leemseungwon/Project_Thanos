using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerMoveState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerAnimation _playerAnimation;
        
        public PlayerMoveState(BaseEntity entity)
            : base(entity)
        {
            _player = entity as PlayerController;
            if (Animation != null)
            {
                _playerAnimation = Animation as PlayerAnimation;
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter();
            
            if(_playerAnimation != null)
                _player.Animation.SetParam(_playerAnimation.MoveAnimationData, true);
        }

        public override void Update()
        {
            Vector2 moveInput = _player.Input.MoveInput;
            
            if (moveInput.sqrMagnitude <= 0.01f)
            {
                _player.StateMachine.ChangeState(PlayerController.IdleState);
                return;
            }

            _player.Movement.SetMoveDirection(moveInput);
        }

        public override void FixedUpdate()
        {
            _player.Movement.Move();
        }

        public override void Exit()
        {
            base.Exit();
            _player.Movement.Stop();
        }
    }
}