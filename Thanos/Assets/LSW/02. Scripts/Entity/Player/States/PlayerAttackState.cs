namespace LSW._02._Scripts.Entity.Player.States
{
    public class PlayerAttackState : EntityState
    {
        private readonly PlayerController _player;
        private bool _attacked;

        public PlayerAttackState(BaseEntity player)
            : base(player)
        {
            _player = player as PlayerController;
        }

        public override void Enter()
        {
            _attacked = false;

            _player.Movement.Stop();

            _player.Combat.Attack(_player.Input.MousePosition);

            _attacked = true;
        }

        public override void Update()
        {
            if (!_attacked)
                return;

            if (_player.Input.MoveInput.sqrMagnitude > 0.01f)
            {
                _player.StateMachine.ChangeState("PlayerMoveState");
            }
            else
            {
                _player.StateMachine.ChangeState("PlayerIdleState");
            }
        }
    }
}