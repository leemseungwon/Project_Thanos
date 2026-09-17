
using LSW._02._Scripts.Environment.Interactable;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerInteractionComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Interaction")]
        [SerializeField] private float interactionRadius = 2f;
        [SerializeField] private LayerMask interactionLayer;
        
        private PlayerController _playerController;
        
        public void Initialize(BaseEntity owner)
        {
            _playerController = owner as PlayerController;
            if (_playerController != null)
            {
                _playerController.Input.OnInteractionPressed += TryInteract;
            }
        }

        private void TryInteract()
        {
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(
                    transform.position,
                    interactionRadius,
                    interactionLayer);

            if (colliders.Length == 0)
                return;

            float closestDistance = float.MaxValue;
            IInteractable closestInteractable = null;

            foreach (Collider2D col in colliders)
            {
                IInteractable interactable =
                    col.GetComponentInParent<IInteractable>();

                if (interactable == null)
                    continue;

                float distance =
                    (col.transform.position -
                     transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }

            if (closestInteractable == null)
                return;

            closestInteractable.Interact();
        }

        public void Reset() { }
        
        private void OnDestroy()
        {
            if (_playerController != null)
            {
                _playerController.Input.OnInteractionPressed -= TryInteract;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position,
                interactionRadius);
        }
    }
}