using System;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Environment.Interactable;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.System.RoomSystems;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public abstract class BaseRoom : MonoBehaviour, IPoolable
    {
        public abstract RoomType Type { get; }
        
        [Header("Base Room Components")]
        [SerializeField] protected Portal portal;
        [SerializeField] protected GameObject portalObject;

        public event Action OnRoomCleared;

        protected bool _isCleared;
        public bool IsCleared => _isCleared;

        public virtual void Initialize()
        {
            _isCleared = false;
            if (portal == null)
                portal = GetComponentInChildren<Portal>(true);
            if (portalObject == null && portal != null)
                portalObject = portal.gameObject;
        }

        public virtual void Spawn()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            _isCleared = false;
            
            if (portal != null)
                portal.gameObject.SetActive(false);
            if (portalObject != null)
                portalObject.SetActive(false);
        }

        public virtual void Despawn()
        {
            if (portal != null)
                portal.gameObject.SetActive(false);
            if (portalObject != null)
                portalObject.SetActive(false);
        }

        public virtual void ClearRoom()
        {
            if (_isCleared) return;
            
            _isCleared = true;
            
            if (portal != null)
                portal.gameObject.SetActive(true);
            if (portalObject != null)
                portalObject.SetActive(true);

            if (SystemHandler.Instance != null && SystemHandler.Instance.GetSystem(out RoomSystem roomSystem))
            {
                roomSystem.ClearCurrentRoom();
            }

            OnRoomCleared?.Invoke();
        }

        protected PlayerController GetPlayer()
        {
            if (SystemHandler.Instance != null && SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem))
            {
                return playerSystem.Player;
            }
            return FindFirstObjectByType<PlayerController>();
        }

        protected PoolSystem GetPoolSystem()
        {
            if (SystemHandler.Instance != null && SystemHandler.Instance.GetSystem(out PoolSystem poolSystem))
            {
                return poolSystem;
            }
            return null;
        }
    }
}