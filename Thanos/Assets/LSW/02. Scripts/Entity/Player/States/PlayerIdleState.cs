

using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerIdleState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerAnimation _playerAnimation;
        
        public PlayerIdleState(BaseEntity player)
            : base(player)
        {
            _player = player as PlayerController;
            if (Animation != null)
            {
                _playerAnimation = Animation as PlayerAnimation;
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            
            _player.Movement.Stop();
            
            if(_playerAnimation != null)
                _player.Animation.SetParam(_playerAnimation.MoveAnimationData, false);
        }

        public override void Update()
        {
            Vector2 moveInput = _player.Input.MoveInput;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                _player.StateMachine.ChangeState(PlayerController.MoveState);
                return;
            }
        }
    }
}