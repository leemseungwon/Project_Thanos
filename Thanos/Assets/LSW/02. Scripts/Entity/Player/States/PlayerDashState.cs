using System;
using LSW._03._So.Entity_Stats.Player;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerDashState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerStatData _playerStatData;
        private readonly PlayerAnimation _playerAnimation;
        
        private float _timer;

        public PlayerDashState(BaseEntity player)
            : base(player)
        {
            _player = player as PlayerController;
            _playerStatData = player.StatData as PlayerStatData;
            if (Animation != null)
            {
                _playerAnimation = Animation as PlayerAnimation;
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            
            if(_playerAnimation != null)
                _player.Animation.SetParam(_playerAnimation.DashAnimationData);
            
            _timer = _playerStatData.dashDuration;

            Vector2 direction = _player.LookDirection;

            if (direction.sqrMagnitude <= 0.01f)
                direction = Vector2.right;

            _player.Movement.Dash(direction, _playerStatData.dashSpeed);
        }
    
        public override void Update()
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                _player.StateMachine.ChangeState(PlayerController.MoveState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            _player.Movement.Stop();
        }
    }
}