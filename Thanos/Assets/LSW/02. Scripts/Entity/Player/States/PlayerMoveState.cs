using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerMoveState : EntityState
    {
        private readonly PlayerController _player;
        
        public PlayerMoveState(BaseEntity entity)
            : base(entity)
        {
            _player = entity as PlayerController;
        }

        public override void Update()
        {
            Vector2 moveInput = _player.Input.MoveInput;
            
            if (moveInput.sqrMagnitude <= 0.01f)
            {
                _player.StateMachine.ChangeState("PlayerIdleState");
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
            _player.Movement.Stop();
        }
    }
}