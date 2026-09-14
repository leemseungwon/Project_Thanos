using LSW._02._Scripts.UI.InGame;
using UnityEngine;

namespace LSW._02._Scripts.Environment.Interactable
{
    public class Portal : MonoBehaviour, IInteractable
    {
        [SerializeField] private RoomSelectUI roomSelectUI;
        
        public void Interact()
        {
            roomSelectUI.Open();
        }
    }
}