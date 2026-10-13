using LSW._02._Scripts.Common;
using LSW._02._Scripts.Rooms;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.System.RoomSystems
{
    [SystemOrder(-19)]
    public class RoomRuntimeSystem : MonoBehaviour, ISystem
    {
        [Header("Room Pool")] 
        [SerializeField] private string startRoomPool = "Room_Start";
        [SerializeField] private string normalRoomPool = "Room_Normal";
        [SerializeField] private string eliteRoomPool = "Room_Elite";
        [SerializeField] private string rewardRoomPool = "Room_Reward";
        [SerializeField] private string bossRoomPool = "Room_Boss";
        
        [Header("Parent")]
        [SerializeField] private Transform roomParent;

        private PoolSystem _poolSystem;
        private RoomNode _loadedRoom;
        private bool _loadedRoomIsRuntime;

        public void Initialize(SystemHandler systemHandler)
        {
            if (!systemHandler.GetSystem(out _poolSystem))
            {
                Debug.LogError("Cannot find PoolSystem.");
            }
        }
        
        public void LoadRoom(RoomNode room)
        {
            if (room == null || _poolSystem == null)
                return;

            UnloadCurrentRoom();

            bool wasCleared = room.Cleared;
            BaseRoom baseRoom = null;
            string poolName = GetPoolName(room.Type);
            if (!string.IsNullOrEmpty(poolName))
                _poolSystem.SpawnPool(poolName, out baseRoom);

            if (baseRoom == null)
                baseRoom = CreateRuntimeRoom(room.Type);

            if (baseRoom != null)
            {
                baseRoom.transform.SetParent(roomParent);
                room.RuntimeObject = baseRoom;
                _loadedRoom = room;

                baseRoom.OnRoomCleared += HandleRoomCleared;
                if (wasCleared)
                {
                    baseRoom.OnReenterClearedRoom();
                    baseRoom.ClearRoom();
                    if (_loadedRoomIsRuntime &&
                        SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem))
                        playerSystem.InGameUI?.RoomSelectUI?.Open();
                }
                else
                    baseRoom.StartRooms();
            }
        }

        private BaseRoom CreateRuntimeRoom(RoomType type)
        {
            GameObject roomObject = new GameObject($"Runtime_{type}Room");
            BaseRoom room = type switch
            {
                RoomType.Shop => roomObject.AddComponent<ShopRoom>(),
                RoomType.Event => roomObject.AddComponent<EventRoom>(),
                _ => null
            };

            if (room == null)
            {
                Destroy(roomObject);
                return null;
            }

            _loadedRoomIsRuntime = true;
            room.Initialize();
            room.Spawn();
            return room;
        }

        public void UnloadCurrentRoom()
        {
            if (_loadedRoom == null || _poolSystem == null)
                return;

            if (_loadedRoom.RuntimeObject != null)
            {
                BaseRoom loadedObject = _loadedRoom.RuntimeObject;
                loadedObject.OnRoomCleared -= HandleRoomCleared;
                if (_loadedRoomIsRuntime)
                    Destroy(loadedObject.gameObject);
                else
                    _poolSystem.DespawnPool(GetPoolName(_loadedRoom.Type), loadedObject);
                _loadedRoom.RuntimeObject = null;
            }

            _loadedRoom = null;
            _loadedRoomIsRuntime = false;
        }

        private void HandleRoomCleared()
        {
            if (SystemHandler.Instance != null && SystemHandler.Instance.GetSystem(out RoomSystem roomSystem))
            {
                roomSystem.ClearCurrentRoom();
            }
        }

        private string GetPoolName(RoomType type)
        {
            return type switch
            {
                RoomType.Start => startRoomPool,
                RoomType.Normal => normalRoomPool,
                RoomType.Elite => eliteRoomPool,
                RoomType.Reward => rewardRoomPool,
                RoomType.Shop => "Room_Shop",
                RoomType.Event => "Room_Event",
                RoomType.Boss => bossRoomPool,
                _ => null
            };
        }

        public void Reset() { }
    }
}
