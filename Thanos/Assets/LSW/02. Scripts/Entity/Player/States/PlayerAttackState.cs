using System;
using LSW._02._Scripts.Entity.Player.Component;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerAttackState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerCombatComponent _combat;

        public PlayerAttackState(BaseEntity player)
            : base(player)
        {
            _player = player as PlayerController;

            if (_player != null)
            {
                _combat = _player.Combat;

                if (_combat != null)
                {
                    _combat.OnComboFinished += HandleComboFinished;
                }
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);

            _player.Movement.EnableMovement(false);

            if (_combat.IsAttacking)
                return;
            
            if (!_combat.TryAttack())
            {
                ChangeToNextState();
            }
        }

        private void HandleComboFinished()
        {
            ChangeToNextState();
        }

        private void ChangeToNextState()
        {
            if (_player.Input.MoveInput.sqrMagnitude > 0.01f)
            {
                _player.StateMachine.ChangeState(PlayerController.MoveState);
            }
            else
            {
                _player.StateMachine.ChangeState(PlayerController.IdleState);
            }
        }

        public override void Update()
        {
            if (_combat.IsAttacking)
                return;
            
            if (_combat.IsWaitingForNextAttack)
            {
                ChangeToNextState();
            }
        }

        public override void Exit()
        {
            base.Exit();

            _player.Movement.EnableMovement(true);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_combat != null)
            {
                _combat.OnComboFinished -= HandleComboFinished;
            }
        }
    }
}