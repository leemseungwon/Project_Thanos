using LSW._03._So.Entity_Stats.Player;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerDashState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerStatData _playerStatData;
        
        private float _timer;

        public PlayerDashState(BaseEntity player)
            : base(player)
        {
            _player = (PlayerController) player;
            _playerStatData = (PlayerStatData) player.StatData;
        }

        public override void Enter()
        {
            _timer = _playerStatData.dashDuration;

            Vector2 direction = _player.LookDirection;

            if (direction.sqrMagnitude <= 0.01f)
                direction = Vector2.right;

            _player.Movement.Dash(
                direction,
                _playerStatData.dashSpeed
            );
        }
    
        public override void Update()
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                _player.StateMachine.ChangeState("PlayerMoveState");
            }
        }

        public override void Exit()
        {
            _player.Movement.Stop();
        }
    }
}