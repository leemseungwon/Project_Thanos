
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.Components
{
    public class EnemyMovementComponent : MonoBehaviour, IEntityComponent
    {
        [SerializeField] protected float stopChaseDistance = 1.5f;
        
        private BaseEnemy _owner;
        private Rigidbody2D _rigidbody;
        
        private bool _isEnabledMovement = true;

        public bool IsPlayerInStopRange =>
            Vector2.Distance(_owner.transform.position, _owner.Player.transform.position) <= stopChaseDistance;

        public void Initialize(BaseEntity owner)
        {
            _owner = owner as BaseEnemy;
            if(_owner != null)
                _rigidbody = owner.GetComponent<Rigidbody2D>();
        }

        public bool MoveTo(Vector2 targetPosition)
        {
            if(!_isEnabledMovement || _owner.IsStunned)
                return false;
            
            if (_owner == null)
                return false;

            Vector2 currentPosition = _owner.transform.position;

            Vector2 direction =
                targetPosition - currentPosition;

            if (IsPlayerInStopRange)
            {
                Stop();
                return false;
            }

            direction.Normalize();

            Vector2 nextPosition = currentPosition +
                                   direction * (_owner.CurrentMoveSpeed * Time.deltaTime);

            if (_rigidbody != null)
                _rigidbody.MovePosition(nextPosition);
            else
                _owner.transform.position = nextPosition;

            return true;
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