using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Rooms;
using UnityEngine;

namespace LSW._02._Scripts.System.RoomSystems
{
    public class RoomNode
    {
        public Vector2Int Coordinate { get; }

        public RoomType Type { get; set; }

        public bool Visited { get; set; }

        public bool Cleared { get; set; }

        // 실제 생성되어 있는 방
        public BaseRoom RuntimeObject { get; set; }

        private readonly List<RoomNode> _neighbors = new();

        public RoomNode(
            Vector2Int coordinate,
            RoomType type)
        {
            Coordinate = coordinate;
            Type = type;
        }

        public void AddNeighbor(RoomNode room)
        {
            if (room == null)
                return;

            if (_neighbors.Contains(room))
                return;

            _neighbors.Add(room);
        }

        public IReadOnlyList<RoomNode> Neighbors
            => _neighbors;
    }
}