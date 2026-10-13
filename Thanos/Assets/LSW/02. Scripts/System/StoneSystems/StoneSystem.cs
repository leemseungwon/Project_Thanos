using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._03._So.Stone_Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSW._02._Scripts.System.StoneSystems
{
    public class StoneSystem : MonoBehaviour, ISystem
    {
        [SerializeField] private Dictionary<BaseStone, List<BaseStoneUpgrade>> stoneDataList;

        private StoneSystem stoneSystem;

        public void Initialize(SystemHandler systemHandler)
        {
            systemHandler.GetSystem(out stoneSystem);
        }

        #region Test

        public void TestGetRandomStone()
        {

        }

        #endregion

        public BaseStone GetStone(StoneType stoneType)
        {
            if (stoneDataList == null)
                return null;

            foreach (BaseStone stone in stoneDataList.Keys)
            {
                if (stone.StoneType == stoneType)
                    return stone;
            }
            return null;
        }

        public BaseStoneUpgrade GetRandomStoneUpgrade()
        {
            if (stoneDataList == null)
                return null;

            List<BaseStoneUpgrade> availableUpgrades = new();
            foreach (List<BaseStoneUpgrade> upgrades in stoneDataList.Values)
            {
                if (upgrades == null)
                    continue;

                foreach (BaseStoneUpgrade upgrade in upgrades)
                {
                    if (upgrade != null)
                        availableUpgrades.Add(upgrade);
                }
            }

            if (availableUpgrades.Count == 0)
                return null;

            return availableUpgrades[Random.Range(0, availableUpgrades.Count)];
        }

        public BaseStoneUpgrade GetRandomStoneUpgrade(PlayerStoneComponent playerStones)
        {
            if (stoneDataList == null || playerStones == null)
                return null;

            List<BaseStoneUpgrade> available = new();
            foreach (List<BaseStoneUpgrade> upgrades in stoneDataList.Values)
            {
                if (upgrades == null) continue;
                foreach (BaseStoneUpgrade upgrade in upgrades)
                {
                    if (upgrade != null && playerStones.HasStone(upgrade.StoneType))
                        available.Add(upgrade);
                }
            }

            return available.Count == 0 ? null : available[Random.Range(0, available.Count)];
        }

        public BaseStoneUpgrade GetRandomStoneUpgrade(StoneType stone)
        {
            BaseStone stoneData = GetStone(stone);
            if (stoneData == null || stoneDataList == null ||
                !stoneDataList.TryGetValue(stoneData, out List<BaseStoneUpgrade> upgrades) ||
                upgrades == null || upgrades.Count == 0)
                return null;

            return upgrades[Random.Range(0, upgrades.Count)];
        }

        public BaseStone GetRandomStone()
        {
            if (stoneDataList == null || stoneDataList.Count == 0)
                return null;

            List<BaseStone> stoneList = new();
            foreach (BaseStone stone in stoneDataList.Keys)
            {
                if (stone != null)
                    stoneList.Add(stone);
            }

            if (stoneList.Count == 0)
                return null;

            return stoneList[Random.Range(0, stoneList.Count)];
        }

        public BaseStone GetRandomUnownedStone(PlayerStoneComponent playerStones)
        {
            if (stoneDataList == null || playerStones == null)
                return null;

            List<BaseStone> available = new();
            foreach (BaseStone stone in stoneDataList.Keys)
            {
                if (stone != null && !playerStones.HasStone(stone.StoneType))
                    available.Add(stone);
            }

            return available.Count == 0 ? null : available[Random.Range(0, available.Count)];
        }

        public bool HasCompleteSet(PlayerStoneComponent playerStones)
        {
            if (playerStones == null) return false;
            foreach (StoneType type in Enum.GetValues(typeof(StoneType)))
                if (!playerStones.HasStone(type)) return false;
            return true;
        }

        public void Reset() { }
    }
}
