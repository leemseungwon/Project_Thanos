
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.UI.InGame;
using UnityEngine;
using IPoolable = LSW._02._Scripts.System.PoolSystems.IPoolable;

namespace LSW._02._Scripts.Environment.Interactable
{
    public class Portal : MonoBehaviour, IInteractable, IPoolable
    {
        private HandlePlayerSystem _handlePlayerSystem;
        private RoomSelectUI _roomSelectUI;
        
        private void Awake()
        {
            ResolveRoomSelectUI();
        }
        
        public void Initialize() { }

        public void Spawn() { }

        public void Interact()
        {
            // Portals are prewarmed by PoolSystem, so Awake can run before
            // scene systems/UI references are ready. Resolve again on use.
            if (_roomSelectUI == null)
                ResolveRoomSelectUI();

            if (_roomSelectUI != null)
            {
                _roomSelectUI.Open();
            }
        }

        private void ResolveRoomSelectUI()
        {
            if (SystemHandler.Instance == null ||
                !SystemHandler.Instance.GetSystem(out _handlePlayerSystem) ||
                _handlePlayerSystem.InGameUI == null)
                return;

            _roomSelectUI = _handlePlayerSystem.InGameUI.RoomSelectUI;
        }


        public void Despawn() { }
    }
}
