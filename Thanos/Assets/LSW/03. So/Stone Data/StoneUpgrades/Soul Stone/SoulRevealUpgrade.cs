using System.Collections.Generic;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.RoomSystems;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Soul_Stone
{
    [CreateAssetMenu(fileName = "Soul Reveal", menuName = "So/Data/Stone Data/Stone Upgrade/Soul Stone Upgrade/Soul Reveal", order = 0)]
    public class SoulRevealUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float bonusDamageMultiplier = 1.2f;
        [SerializeField] private float duration = 3f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if(SystemHandler.Instance.GetSystem(out RoomSystem roomSystem))
            {
                List<BaseEnemy> spawnedObjects = roomSystem.GetSpawnedObjects();
                if(spawnedObjects.Count > 0)
                    return;

                foreach (var spawnedObject in spawnedObjects)
                {
                    if(spawnedObject.GetCompo(out EntityBuffComponent buffCompo))
                    {
                        buffCompo.RevealHealthBar();
                        buffCompo.ApplyIncomingDamageMultiplier("Soul Reveal", bonusDamageMultiplier, duration);
                    }
                }
            }
        }
    }
}