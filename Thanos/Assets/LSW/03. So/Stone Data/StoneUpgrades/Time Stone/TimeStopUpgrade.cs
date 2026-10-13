using System.Collections.Generic;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.RoomSystems;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Time_Stone
{
    [CreateAssetMenu(fileName = "Time Stop Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Time Stone Upgrade/Time Stop", order = 0)]
    public class TimeStopUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float duration = 2f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null || duration <= 0f ||
                SystemHandler.Instance == null ||
                !SystemHandler.Instance.GetSystem(out RoomSystem roomSystem))
            {
                return;
            }

            List<BaseEnemy> spawnedObjects = roomSystem.GetSpawnedObjects();
            if (spawnedObjects == null || spawnedObjects.Count == 0)
                return;

            foreach (BaseEnemy spawnedObject in spawnedObjects)
            {
                if (spawnedObject == null || !spawnedObject.gameObject.activeInHierarchy ||
                    spawnedObject.IsDead)
                {
                    continue;
                }

                if (spawnedObject.GetCompo(out EntityBuffComponent buffCompo))
                {
                    buffCompo.ApplyActionDisable(duration);
                }
            }
        }
    }
}