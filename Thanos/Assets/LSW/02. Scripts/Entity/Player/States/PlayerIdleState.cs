

using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerIdleState : EntityState
    {
        private readonly PlayerController _player;
        
        public PlayerIdleState(BaseEntity player)
            : base(player)
        {
            _player = player as PlayerController;
        }

        public override void Enter()
        {
            _player.Movement.Stop();
        }

        public override void Update()
        {
            Vector2 moveInput = _player.Input.MoveInput;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                _player.StateMachine.ChangeState("PlayerMoveState");
                return;
            }
        }
    }
}