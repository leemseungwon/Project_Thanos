using System;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSW._02._Scripts.System.RoomSystems
{
    [Serializable]
    public class RoomSpawnWeight
    {
        public RoomType roomType;

        [Min(0f)]
        public float weight = 10f;
    }
    
    
    [SystemOrder(-18)]
    public class RoomSystem : MonoBehaviour, ISystem
    {
        [Header("Map")]
        [SerializeField] private int roomCount = 30;

        [Header("Room Type Weight")] 
        [SerializeField]
        private List<RoomSpawnWeight> roomWeights = new()
        {
            new RoomSpawnWeight
            {
                roomType = RoomType.Normal,
                weight = 50f
            },

            new RoomSpawnWeight
            {
                roomType = RoomType.Elite,
                weight = 10f
            },

            new RoomSpawnWeight
            {
                roomType = RoomType.Reward,
                weight = 10f
            },
            new RoomSpawnWeight
            {
                roomType = RoomType.Shop,
                weight = 10f
            },
            new RoomSpawnWeight
            {
                roomType = RoomType.Event,
                weight = 10f
            }
        };

        private readonly Dictionary<Vector2Int, RoomNode> _rooms
            = new();

        private RoomRuntimeSystem _roomRuntimeSystem;
        private RoomNode _startRoom;
        private RoomNode _bossRoom;

        public RoomNode CurrentRoom { get; private set; }
        public bool IsRunWon { get; private set; }
        public event Action OnRunWon;

        public RoomNode StartRoom => _startRoom;

        public RoomNode BossRoom => _bossRoom;

        public Dictionary<Vector2Int, RoomNode> Rooms
            => _rooms;

        private static readonly Vector2Int[] HexDirections =
        {
            new(1, 0),
            new(1, -1),
            new(0, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1)
        };

        private void Start()
        {
            if (SystemHandler.Instance.GetSystem(out _roomRuntimeSystem))
            {
                _roomRuntimeSystem.LoadRoom(_startRoom);
            }
        }

        public void GenerateMap()
        {
            _rooms.Clear();
            IsRunWon = false;

            // ���۹�
            _startRoom = CreateRoom(Vector2Int.zero, RoomType.Start);

            CurrentRoom = _startRoom;

            // BFS
            GenerateRoomGraph();

            // �� Ÿ�� ����
            AssignRoomTypes();

            Debug.Log(
                $"Room Map Generated : {_rooms.Count}");
        }

        // =========================================================
        // BFS Graph
        // =========================================================

        private void GenerateRoomGraph()
        {
            Queue<RoomNode> queue = new();

            queue.Enqueue(_startRoom);

            while (queue.Count > 0 &&
                   _rooms.Count < roomCount)
            {
                RoomNode current = queue.Dequeue();

                List<Vector2Int> candidates = new();

                foreach (Vector2Int direction in HexDirections)
                {
                    Vector2Int next =
                        current.Coordinate + direction;

                    if (_rooms.ContainsKey(next))
                        continue;

                    candidates.Add(next);
                }

                candidates.Sort(
                    (a, b) =>
                    {
                        int distanceA =
                            GetHexDistance(Vector2Int.zero, a);

                        int distanceB =
                            GetHexDistance(Vector2Int.zero, b);

                        return distanceA.CompareTo(distanceB);
                    });

                ShuffleSameDistance(candidates);

                foreach (Vector2Int coordinate in candidates)
                {
                    if (_rooms.Count >= roomCount)
                        break;

                    RoomNode nextRoom =
                        CreateRoom(
                            coordinate,
                            RoomType.Normal);

                    // ���� ��� �θ�� ����
                    current.AddNeighbor(nextRoom);
                    nextRoom.AddNeighbor(current);

                    queue.Enqueue(nextRoom);
                }
            }

            // �� ������ ��ǥ�� ���� ������ ��� �� ����
            ConnectAllNeighbors();
        }
        
        private void ConnectAllNeighbors()
        {
            foreach (RoomNode room in _rooms.Values)
            {
                foreach (Vector2Int direction in HexDirections)
                {
                    Vector2Int neighborCoordinate =
                        room.Coordinate + direction;

                    if (!_rooms.TryGetValue(
                            neighborCoordinate,
                            out RoomNode neighbor))
                    {
                        continue;
                    }

                    room.AddNeighbor(neighbor);
                    neighbor.AddNeighbor(room);
                }
            }
        }
        
        private int GetHexDistance(Vector2Int a, Vector2Int b)
        {
            int q = a.x - b.x;
            int r = a.y - b.y;

            return (Mathf.Abs(q) + Mathf.Abs(r) +
                    Mathf.Abs(q + r)) / 2;
        }

        private void AssignRoomTypes()
        {
            List<RoomNode> candidates = new();

            foreach (RoomNode room in _rooms.Values)
            {
                if (room == _startRoom)
                    continue;

                candidates.Add(room);
            }

            // ���� �� ���� ������ ����
            _bossRoom =
                FindFarthestRoom();

            if (_bossRoom != null)
            {
                _bossRoom.Type =
                    RoomType.Boss;

                candidates.Remove(_bossRoom);
            }

            // ������ �濡 ����ġ ���� ����
            foreach (RoomNode room in candidates)
            {
                room.Type =
                    GetRandomRoomType();
            }
        }

        private RoomType GetRandomRoomType()
        {
            float totalWeight = 0f;

            foreach (RoomSpawnWeight data in roomWeights)
            {
                if (data.weight > 0f)
                    totalWeight += data.weight;
            }

            if (totalWeight <= 0f)
            {
                Debug.LogWarning(
                    "Room weight is zero. Normal room will be used.");

                return RoomType.Normal;
            }

            float random =
                Random.Range(0f, totalWeight);

            float currentWeight = 0f;

            foreach (RoomSpawnWeight data in roomWeights)
            {
                if (data.weight <= 0f)
                    continue;

                currentWeight += data.weight;

                if (random <= currentWeight)
                    return data.roomType;
            }

            return RoomType.Normal;
        }

        private RoomNode FindFarthestRoom()
        {
            RoomNode farthestRoom = null;

            int maxDistance = -1;

            foreach (RoomNode room in _rooms.Values)
            {
                if (room == _startRoom)
                    continue;

                int distance =
                    GetDistanceFromStart(room);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestRoom = room;
                }
            }

            return farthestRoom;
        }

        private int GetDistanceFromStart(
            RoomNode target)
        {
            Queue<RoomNode> queue = new();

            Dictionary<RoomNode, int> distance =
                new();

            queue.Enqueue(_startRoom);
            distance[_startRoom] = 0;

            while (queue.Count > 0)
            {
                RoomNode current =
                    queue.Dequeue();

                if (current == target)
                    return distance[current];

                foreach (RoomNode neighbor
                         in current.Neighbors)
                {
                    if (distance.ContainsKey(neighbor))
                        continue;

                    distance[neighbor] =
                        distance[current] + 1;

                    queue.Enqueue(neighbor);
                }
            }

            return -1;
        }

        public List<BaseEnemy> GetSpawnedObjects()
            => CurrentRoom.RuntimeObject.SpawnedPoolObjects;
        
        public IReadOnlyList<RoomNode> GetNextRooms()
        {
            if (CurrentRoom == null)
                return null;

            if (!CurrentRoom.Cleared)
                return null;

            List<RoomNode> result = new();

            foreach (RoomNode room in CurrentRoom.Neighbors)
            {
                if (room == null)
                    continue;

                result.Add(room);
            }

            return result;
        }
        
        public void SelectNextRoom(RoomNode nextRoom)
        {
            if (CurrentRoom == null)
                return;

            if (nextRoom == null)
                return;

            if (!CurrentRoom.Neighbors.Contains(nextRoom))
                return;

            CurrentRoom = nextRoom;
            CurrentRoom.Visited = true;

            _roomRuntimeSystem.LoadRoom(CurrentRoom);
        }
        
        [ContextMenu("Clear Current Room")]
        public void ClearCurrentRoom()
        {
            if (CurrentRoom == null)
                return;

            CurrentRoom.Cleared = true;
            if (CurrentRoom == _bossRoom && !IsRunWon)
            {
                IsRunWon = true;
                OnRunWon?.Invoke();
            }
        }

        private RoomNode CreateRoom( Vector2Int coordinate, RoomType type)
        {
            RoomNode room = new RoomNode(coordinate,
                    type);

            _rooms.Add(coordinate, room);

            return room;
        }

        private void ShuffleSameDistance<T>(List<T> list)
        {
            for (int i = list.Count - 1;
                 i > 0;
                 i--)
            {
                int random =
                    Random.Range(0, i + 1);

                (list[i], list[random]) =
                    (list[random], list[i]);
            }
        }

        public void Initialize(SystemHandler systemHandler)
        {
            GenerateMap();
            _startRoom.Visited = true;
        }

        public void Reset()
        {
            IsRunWon = false;
        }
    }
}
