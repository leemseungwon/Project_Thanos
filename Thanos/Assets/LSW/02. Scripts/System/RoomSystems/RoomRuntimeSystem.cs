using LSW._02._Scripts.Common;
using LSW._02._Scripts.Rooms;
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
        [SerializeField] private string shopRoomPool = "Room_Shop";
        [SerializeField] private string eliteRoomPool = "Room_Elite";
        [SerializeField] private string rewardRoomPool = "Room_Reward";
        [SerializeField] private string eventRoomPool = "Room_Event";
        [SerializeField] private string bossRoomPool = "Room_Boss";
        
        [Header("Parent")]
        [SerializeField] private Transform roomParent;

        private PoolSystem _poolSystem;
        private RoomNode _loadedRoom;

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

            if (_poolSystem.SpawnPool(GetPoolName(room.Type), out BaseRoom baseRoom))
            {
                baseRoom.transform.SetParent(roomParent);
                room.RuntimeObject = baseRoom;
                _loadedRoom = room;
            }
        }

        public void UnloadCurrentRoom()
        {
            if (_loadedRoom == null || _poolSystem == null)
                return;

            if (_loadedRoom.RuntimeObject != null)
            {
                _poolSystem.DespawnPool(GetPoolName(_loadedRoom.Type), _loadedRoom.RuntimeObject);
                _loadedRoom.RuntimeObject = null;
            }

            _loadedRoom = null;
        }

        private string GetPoolName(RoomType type)
        {
            return type switch
            {
                RoomType.Start => startRoomPool,
                RoomType.Normal => normalRoomPool,
                RoomType.Shop => shopRoomPool,
                RoomType.Elite => eliteRoomPool,
                RoomType.Reward => rewardRoomPool,
                RoomType.Event => eventRoomPool,
                RoomType.Boss => bossRoomPool,
                _ => null
            };
        }

        public void Reset() { }
    }
}