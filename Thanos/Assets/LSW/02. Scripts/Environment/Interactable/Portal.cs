
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.UI.InGame;
using UnityEngine;

namespace LSW._02._Scripts.Environment.Interactable
{
    public class Portal : MonoBehaviour, IInteractable
    {
        private HandlePlayerSystem _handlePlayerSystem;
        private RoomSelectUI _roomSelectUI;
        
        private void Awake()
        {
            if(SystemHandler.Instance.GetSystem(out _handlePlayerSystem))
            {
                _roomSelectUI = _handlePlayerSystem.InGameUI.RoomSelectUI;
            }
        }

        public void Interact()
        {
            if (_roomSelectUI != null)
            {
                _roomSelectUI.Open();
            }
        }
    }
}