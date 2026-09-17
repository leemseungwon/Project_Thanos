using System;
using System.Collections.Generic;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Rooms;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Common
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

    [Serializable]
    public class CurrentStoneState
    {
        private Dictionary<StoneType, CurrentStoneData> _currentStoneDataList = new();
        public bool IsEmpty => _currentStoneDataList.Count == 0;

        public void AddStone(StoneType stone, BaseStone stoneData)
        {
            if(_currentStoneDataList.ContainsKey(stone))
                return;
            _currentStoneDataList.Add(stone, new CurrentStoneData(stoneData));
        }
        
        public void AddStoneUpgrade(StoneType baseStone, BaseStoneUpgrade stoneUpgrade)
        {
            if (!_currentStoneDataList.ContainsKey(baseStone))
                return;
            _currentStoneDataList[baseStone].stoneUpgrades.Add(stoneUpgrade);
        }

        public void UseStone(StoneType stone, PlayerController playerController)
        {
            if(!_currentStoneDataList.ContainsKey(stone))
                return;
            _currentStoneDataList[stone].UseStone(playerController);
        }
        
        public bool CanUseStone(StoneType stone)
        {
            return _currentStoneDataList.ContainsKey(stone) && _currentStoneDataList[stone].canUse;
        }

        public void UpdateCanUseStone(StoneType stoneType, bool canUse)
        {
            if(_currentStoneDataList[stoneType] == null)
                return;
            _currentStoneDataList[stoneType].canUse = canUse;
        }
    }
    
    [Serializable]
    public class CurrentStoneData
    {
        public bool canUse = true;
        public BaseStone stone;
        public List<BaseStoneUpgrade> stoneUpgrades = new();

        public CurrentStoneData(BaseStone stoneData)
        {
            stone = stoneData;
        }
        
        public void UseStone(PlayerController playerController)
        {
            canUse = false;
            stone.Use(playerController);
            UseAllUpgrades(playerController);
        }
        
        public void UseAllUpgrades(PlayerController playerController)
        {
            stoneUpgrades.ForEach(stoneUpgrade => stoneUpgrade.OnUse(playerController, stone));
        }
    }
}