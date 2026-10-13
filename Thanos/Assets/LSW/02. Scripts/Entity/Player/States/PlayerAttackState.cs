using System;
using LSW._02._Scripts.Entity.Player.Component;

namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerAttackState : EntityState
    {
        private readonly PlayerController _player;
        private readonly PlayerCombatComponent _combat;

        public PlayerAttackState(PlayerController player)
            : base(player)
        {
            _player = player;

            if (_player != null)
            {
                _combat = _player.Combat;
            }
        }

        public override void Enter(Action endAction = null)
        {
            base.Enter(endAction);
            
            if (!_combat.TryAttack())
            {
                ChangeToNextState();
                return;
            }
            _combat.OnAttackFinished += ChangeToNextState;
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

        public override void Exit()
        {
            base.Exit();
            _combat.OnAttackFinished -= ChangeToNextState;
        }
    }
}