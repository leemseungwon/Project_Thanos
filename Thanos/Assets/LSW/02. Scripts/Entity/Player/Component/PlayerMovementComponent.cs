using LSW._03._So.Stone_Data;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerMovementComponent : MonoBehaviour, IEntityComponent
    {
        public Vector2 MoveDirection { get; private set; }
        public float TotalMoveTime => _totalMoveTime;
        public bool IsEnabledMovement { get; private set; }= true;
        
        private Rigidbody2D _rigidbody;
        private BaseEntity _owner;
        private float _totalMoveTime;
        
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
            if(!IsEnabledMovement)
                return;
            
            _rigidbody.linearVelocity = MoveDirection * _owner.CurrentMoveSpeed;
            _totalMoveTime += Time.deltaTime;
        }

        public void Stop()
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        public void EnableMovement(bool enable)
        {
            if(!enable)
                Stop();
            IsEnabledMovement = enable;
        }

        public void Dash(Vector2 direction, float force)
        {
            if(!IsEnabledMovement)
                return;
            
            _rigidbody.linearVelocity = direction.normalized * force;
        }

        public void ResetMoveTime()
        {
            _totalMoveTime = 0;
        }
        
        public void Reset() { }
    }
}