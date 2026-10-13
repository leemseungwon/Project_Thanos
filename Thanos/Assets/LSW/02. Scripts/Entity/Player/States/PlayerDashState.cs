using System;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._03._So.Entity_Stats.Player;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerDashState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerAnimation _playerAnimation;
        private readonly PlayerStoneComponent _stoneComponent;
        
        private readonly PlayerStatData _playerStatData;
        
        private float _timer;

        public PlayerDashState(PlayerController player) : base(player)
        {
            _player = player;
            _playerStatData = player.StatData as PlayerStatData;
            player.GetCompo(out _stoneComponent);
                
            if (Animation != null)
            {
                _playerAnimation = Animation as PlayerAnimation;
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);

            Vector2 moveInput = _player.Input.MoveInput;
            Vector2 direction = moveInput.sqrMagnitude > 0.01f
                ? moveInput.normalized
                : _player.LookDirection;

            _player.SetLookDirection(direction);

            if (_playerAnimation != null)
                _player.Animation.SetParam(_playerAnimation.DashAnimationData);

            _timer = _playerStatData.dashDuration;
            _player.Movement.Dash(direction, _playerStatData.dashSpeed);

            _stoneComponent.OnPlayerDash();
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
