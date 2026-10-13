

using System;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerIdleState : EntityState
    {
        private readonly PlayerController _player;
        
        public PlayerIdleState(PlayerController player)
            : base(player)
        {
            _player = player;
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            
            _player.Movement.Stop();
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