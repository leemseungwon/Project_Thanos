
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.Components
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyHitPoint : MonoBehaviour
    {
        private CircleCollider2D _collider;
        
        public bool IsInHitPoint { get; set; }

        public void Initialize(float hitPointRadius)
        {
            _collider = GetComponent<CircleCollider2D>();
            if (_collider != null)
            {
                _collider.radius = hitPointRadius;
                _collider.isTrigger = true;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.TryGetComponent(out PlayerController _))
                IsInHitPoint = true;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.TryGetComponent(out PlayerController _))
                IsInHitPoint = false;
        }

        private void OnDrawGizmosSelected()
        {
            if(_collider == null)
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _collider.radius);
        }
    }
}