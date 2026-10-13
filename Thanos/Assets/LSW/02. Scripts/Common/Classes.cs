using System;
using System.Collections.Generic;
using LSW._02._Scripts.Entity;
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

        // ���� �����Ǿ� �ִ� ��
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
        
        public void AddStoneUpgrade(StoneType baseStone, BaseStoneUpgrade stoneUpgrade,
            PlayerController player)
        {
            if (!_currentStoneDataList.ContainsKey(baseStone))
                return;

            _currentStoneDataList[baseStone].stoneUpgrades.Add(stoneUpgrade);

            stoneUpgrade.OnAcquire(player);
        }
        
        public bool HasStone(StoneType stone)
        {
            return _currentStoneDataList.ContainsKey(stone);
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

        public bool HasStoneUpgrade<T>(StoneType stone) where T : BaseStoneUpgrade
        {
            return _currentStoneDataList.TryGetValue(stone, out CurrentStoneData data) &&
                   data.HasUpgrade<T>();
        }

        public bool HasStoneUpgrade(StoneType stone, Type upgradeType)
        {
            return _currentStoneDataList.TryGetValue(stone, out CurrentStoneData data) &&
                   data.HasUpgrade(upgradeType);
        }

        public void UpdateCanUseStone(StoneType stoneType, bool canUse)
        {
            if (!_currentStoneDataList.TryGetValue(stoneType, out CurrentStoneData data))
                return;

            data.canUse = canUse;
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

        public bool HasUpgrade<T>() where T : BaseStoneUpgrade
        {
            return stoneUpgrades.Exists(upgrade => upgrade is T);
        }

        public bool HasUpgrade(Type upgradeType)
        {
            return upgradeType != null &&
                   typeof(BaseStoneUpgrade).IsAssignableFrom(upgradeType) &&
                   stoneUpgrades.Exists(upgrade => upgradeType.IsInstanceOfType(upgrade));
        }
        
        public void UseStone(PlayerController playerController)
        {
            if (!canUse)
                return;

            stone.Use(playerController);
            UseAllUpgrades(playerController);
            canUse = false;
        }
        
        public void UseAllUpgrades(PlayerController playerController)
        {
            stoneUpgrades.ForEach(stoneUpgrade => stoneUpgrade.OnUse(playerController, stone));
        }
    }
    
    [Serializable]
    public class StoneConditionRuntime
    {
        public int Stack;
        public int Charge;
        public int Counter;
        public float Timer;
        public float BlockTimer;
        public bool ReviveAvailable;
    }
    
    [Serializable]
    public class TimedMultiplier
    {
        public string Id;
        public float Multiplier;
        public float RemainingTime;

        public TimedMultiplier(string id, float multiplier, float duration)
        {
            Id = id;
            Multiplier = multiplier;
            RemainingTime = duration;
        }
    }

    [Serializable]
    public class StatModifier
    {
        public string Id;
        public StatModifierType Type;
        public float Multiplier;
        public float RemainingTime;

        public StatModifier(string id, 
            StatModifierType type, float multiplier, float duration)
        {
            Id = id;
            Type = type;
            Multiplier = multiplier;
            RemainingTime = duration;
        }
    }

    public class DamageOverTime
    {
        public string Id;
        public BaseEntity Source;
        public BaseEntity Target;
        public int Damage;
        public float TickInterval;
        public float RemainingTime;
        public float TickTimer;
    }

    public class ShieldData
    {
        public float ReflectionMultiplier;
        public int RemainingUses;

        public bool ChainReflect;
        public float ChainReflectRadius;

        public bool ReflectHeal;
        public int ReflectHealAmount;

        public ShieldData(
            float reflectionMultiplier,
            int remainingUses)
        {
            ReflectionMultiplier = reflectionMultiplier;
            RemainingUses = remainingUses;
        }
    }
    
    public struct ShieldReflectionData
    {
        public int Damage;
        public BaseEntity Attacker;

        public bool ChainReflect;
        public float ChainReflectRadius;

        public bool ReflectHeal;
        public int ReflectHealAmount;
    }
}