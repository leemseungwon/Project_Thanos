
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.Components
{
    public class EnemyMovementComponent : MonoBehaviour, IEntityComponent
    {
        private BaseEntity _owner;
        private Rigidbody2D _rigidbody;
        
        private bool _isEnabledMovement = true;

        public void Initialize(BaseEntity owner)
        {
            _owner = owner;
            _rigidbody = owner.GetComponent<Rigidbody2D>();
        }

        public void MoveTo(Vector2 targetPosition)
        {
            if(!_isEnabledMovement || _owner.IsStunned)
                return;
            
            if (_owner == null)
                return;

            Vector2 currentPosition = _owner.transform.position;

            Vector2 direction =
                targetPosition - currentPosition;

            if (direction.sqrMagnitude <= 0.01f)
            {
                Stop();
                return;
            }

            direction.Normalize();

            Vector2 nextPosition = currentPosition +
                                   direction * (_owner.CurrentMoveSpeed * Time.deltaTime);

            if (_rigidbody != null)
                _rigidbody.MovePosition(nextPosition);
            else
                _owner.transform.position = nextPosition;
        }
        
        public void EnableMovement(bool enable)
        {
            if(!enable)
                Stop();
            _isEnabledMovement = enable;
        }
        
        public void Stop()
        {
            if (_rigidbody != null)
                _rigidbody.linearVelocity = Vector2.zero;
        }
        
        public void Reset() { }
    }
}