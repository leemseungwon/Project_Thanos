using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerMovementComponent : MonoBehaviour, IEntityComponent
    {
        public Vector2 MoveDirection { get; private set; }
        
        private Rigidbody2D _rigidbody;
        private BaseEntity _owner;

        public void Initialize(BaseEntity owner)
        {
            _owner = owner;
            _rigidbody = owner.GetComponent<Rigidbody2D>();
        }

        public void SetMoveDirection(Vector2 direction)
        {
            MoveDirection = direction.normalized;
        }

        public void Move()
        {
            _rigidbody.linearVelocity = MoveDirection * _owner.CurrentMoveSpeed;
        }

        public void Stop()
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        public void Dash(Vector2 direction, float force)
        {
            _rigidbody.linearVelocity = direction.normalized * force;
        }
    }
}