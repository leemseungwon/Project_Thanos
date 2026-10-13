using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Environment.Interactable;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.System.RoomSystems;
using LSW._02._Scripts.UI.InGame;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public abstract class BaseRoom : MonoBehaviour, IPoolable
    {
        public abstract RoomType Type { get; }

        [Header("Base Room Components")]
        [SerializeField] protected string portalPoolName = "Portal";
        [SerializeField] private Transform portalSpawnPoint;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] protected string rewardPickupPoolName = "RewardPickup";

        public bool IsCleared => _isCleared;
        protected bool HasRewardPickups => _rewardPickups.Count > 0;
        public PoolSystem PoolSystem => GetPoolSystem();
        public List<BaseEnemy> SpawnedPoolObjects { get; } = new List<BaseEnemy>();

        protected bool _isCleared;
        private Portal _spawnedPortal;
        private readonly List<RewardPickup> _rewardPickups = new();

        public event Action OnRoomCleared;

        public virtual void Initialize()
        {
            _isCleared = false;
        }

        public virtual void Spawn()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            _isCleared = false;

            if (playerSpawnPoint != null)
            {
                PlayerController player = GetPlayer();
                if (player != null)
                {
                    player.transform.SetPositionAndRotation(playerSpawnPoint.position,
                        playerSpawnPoint.rotation);
                    player.Rigidbody.linearVelocity = Vector2.zero;
                    player.Rigidbody.angularVelocity = 0f;
                }
            }
        }

        public virtual void Despawn()
        {
            DespawnRewardPickups();

            if (_spawnedPortal != null && PoolSystem != null)
            {
                PoolSystem.DespawnPool(portalPoolName, _spawnedPortal);
            }
        }

        public abstract void StartRooms();

        public virtual void OnReenterClearedRoom() { }

        protected bool DropRewardPickup(Action onCollected, bool lockedUntilRoomClear = false,
            Vector3? spawnPosition = null)
        {
            PoolSystem poolSystem = GetPoolSystem();
            if (poolSystem == null ||
                !poolSystem.SpawnPool(rewardPickupPoolName, out RewardPickup pickup))
                return false;

            _rewardPickups.Add(pickup);
            Transform player = GetPlayer()?.transform;
            pickup.transform.SetParent(transform, true);
            Vector3 dropPosition = transform.position;
            if (player != null)
            {
                Vector3 directionToRoomCenter = transform.position - player.position;
                if (directionToRoomCenter.sqrMagnitude <= 0.01f)
                    directionToRoomCenter = Vector3.up;
                dropPosition = player.position + directionToRoomCenter.normalized * 2f;
            }
            Vector3 finalDropPosition = spawnPosition ?? dropPosition;

            pickup.transform.position = finalDropPosition;
            pickup.Configure(poolSystem, rewardPickupPoolName, () =>
            {
                _rewardPickups.Remove(pickup);
                onCollected?.Invoke();
            });
            pickup.SetCollectible(!lockedUntilRoomClear);

            return true;
        }

        protected void UnlockRewardPickups()
        {
            foreach (RewardPickup pickup in _rewardPickups)
            {
                if (pickup != null && pickup.gameObject.activeSelf)
                    pickup.SetCollectible(true);
            }
        }

        private void DespawnRewardPickups()
        {
            PoolSystem poolSystem = GetPoolSystem();
            if (poolSystem != null)
            {
                foreach (RewardPickup pickup in _rewardPickups)
                {
                    if (pickup != null && pickup.gameObject.activeSelf)
                        poolSystem.DespawnPool(rewardPickupPoolName, pickup);
                }
            }
            _rewardPickups.Clear();
        }

        public virtual void ClearRoom()
        {
            if (_isCleared) return;

            _isCleared = true;

            if (SystemHandler.Instance != null && SystemHandler.Instance.GetSystem(out RoomSystem roomSystem))
            {
                roomSystem.ClearCurrentRoom();
            }

            if (PoolSystem != null && portalSpawnPoint != null)
            {
                if (PoolSystem.SpawnPool(portalPoolName, out _spawnedPortal))
                {
                    _spawnedPortal.transform.SetParent(portalSpawnPoint);
                    _spawnedPortal.transform.position = portalSpawnPoint.position;
                }
            }
            OnRoomCleared?.Invoke();
        }

        protected HandlePlayerSystem GetPlayerSystem()
        {
            if (SystemHandler.Instance != null && SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem))
            {
                return playerSystem;
            }
            return null;
        }

        protected PlayerController GetPlayer()
        {
            HandlePlayerSystem playerSystem = GetPlayerSystem();
            if (playerSystem != null)
            {
                return playerSystem.Player;
            }
            return null;
        }

        protected InGameUI GetInGameUI()
        {
            HandlePlayerSystem playerSystem = GetPlayerSystem();
            if (playerSystem != null)
            {
                return playerSystem.InGameUI;
            }
            return null;
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
