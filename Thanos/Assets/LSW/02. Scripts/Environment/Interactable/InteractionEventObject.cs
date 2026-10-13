using UnityEngine;
using UnityEngine.Events;

namespace LSW._02._Scripts.Environment.Interactable
{
    public class InteractionEventObject : MonoBehaviour, IInteractable
    {
        public UnityEvent onInteract;
        
        public void Interact()
        {
            onInteract?.Invoke();
        }
    }
}