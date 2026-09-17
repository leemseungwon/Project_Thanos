using System;
using LSW._02._Scripts.Entity.Player.Component;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerParryState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerAnimation _playerAnimation;
        private readonly PlayerMovementComponent _movementComponent;
        private readonly PlayerParryComponent _parryComponent;
        
        private bool _isPlayingEndAnimation;
        
        private string _idleStateName;

        public PlayerParryState(BaseEntity player)
            : base(player)
        {
            _player = player as PlayerController;

            if (_player != null)
            {
                _movementComponent = _player.Movement;
                _parryComponent = _player.Parry;

                if (_parryComponent != null)
                {
                    _parryComponent.OnParryEnd += HandleParryEnd;
                }
            }

            if (Animation != null)
            {
                _playerAnimation = Animation as PlayerAnimation;

                if (_playerAnimation != null)
                {
                    _playerAnimation.OnParryAnimationEnd += HandleParryAnimationEnd;
                }
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            
            _isPlayingEndAnimation = false;

            if (!_player.Parry.TryStartParry())
            {
                _player.StateMachine.ChangeState(PlayerController.IdleState);
                return;
            }
            
            _movementComponent.EnableMovement(false);

            if (_playerAnimation != null)
            {
                _player.Animation.SetParam(
                    _playerAnimation.ParryAnimationData
                );
            }
        }
        
        private void HandleParryEnd(bool isSuccess)
        {
            _isPlayingEndAnimation = true;
            
            _movementComponent.EnableMovement(false);

            if (_playerAnimation != null)
            {
                _player.Animation.SetParam(isSuccess ? _playerAnimation.IsParrySuccessAnimationData : _playerAnimation.IsParryFailAnimationData);
            }
        }

        private void HandleParryAnimationEnd()
        {
            if (!_isPlayingEndAnimation)
                return;

            _isPlayingEndAnimation = false;

            _player.StateMachine.ChangeState(PlayerController.IdleState);
        }

        public override void Update()
        {
            if (_isPlayingEndAnimation)
                return;

            if (!_player.Parry.IsParrying)
            {
                _player.StateMachine.ChangeState(PlayerController.IdleState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            _movementComponent.EnableMovement(true);

            _isPlayingEndAnimation = false;
        }

        public override void Dispose()
        {
            if (_parryComponent != null)
            {
                _parryComponent.OnParryEnd -= HandleParryEnd;
            }

            if (_playerAnimation != null)
            {
                _playerAnimation.OnParryAnimationEnd -= HandleParryAnimationEnd;
            }
        }
    }
}