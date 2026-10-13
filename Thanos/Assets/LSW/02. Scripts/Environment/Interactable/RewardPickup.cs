using System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.Environment.Interactable
{
    public class RewardPickup : MonoBehaviour, IInteractable, IPoolable
    {
        private PoolSystem _poolSystem;
        private string _poolName;
        private Action _onCollected;
        private bool _canCollect;

        public bool CanCollect => _canCollect && _onCollected != null;

        public void Initialize() { }

        public void Spawn()
        {
            _onCollected = null;
            _canCollect = false;
        }

        public void Configure(PoolSystem poolSystem, string poolName, Action onCollected)
        {
            _poolSystem = poolSystem;
            _poolName = poolName;
            _onCollected = onCollected;
        }

        public void SetCollectible(bool canCollect)
        {
            _canCollect = canCollect;
        }

        public void Interact()
        {
            Action callback = _onCollected;
            if (callback == null || !CanCollect)
                return;

            _onCollected = null;
            _canCollect = false;
            if (_poolSystem != null)
                _poolSystem.DespawnPool(_poolName, this);
            else
                gameObject.SetActive(false);

            callback.Invoke();
        }

        public void Despawn()
        {
            _onCollected = null;
            _canCollect = false;
        }
    }
}
