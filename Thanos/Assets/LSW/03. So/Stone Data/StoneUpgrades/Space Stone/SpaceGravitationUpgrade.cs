using System.Collections;
using System.Collections.Generic;
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.RoomSystems;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades.Space_Stone
{
    [CreateAssetMenu(fileName = "Space Gravitation Upgrade", menuName = "So/Data/Stone Data/Stone Upgrade/Space Stone Upgrade/Space Gravitation", order = 0)]
    public class SpaceGravitationUpgrade : BaseStoneUpgrade
    {
        [SerializeField, Min(0f)] private float pullSpeed = 5f;
        [SerializeField, Min(0f)] private float pullDuration = 2f;
        
        public override void OnUse(PlayerController player, BaseStone stone)
        {
            if (player == null || stone is not SpaceStone spaceStone ||
                !spaceStone.DidTeleportOnLastUse || pullDuration <= 0f)
                return;

            player.StartCoroutine(PullEnemies(spaceStone.LastTeleportDestination, pullDuration));
        }

        private IEnumerator PullEnemies(Vector2 center, float duration)
        {
            float remainingDuration = duration;
            while (remainingDuration > 0f)
            {
                if (SystemHandler.Instance.GetSystem(out RoomSystem roomSystem))
                {
                    List<BaseEnemy> spawnedObjects = roomSystem.GetSpawnedObjects();
                    
                    if (spawnedObjects != null && spawnedObjects.Count > 0)
                    {
                        foreach (BaseEnemy enemy in spawnedObjects)
                        {
                            if (enemy == null || enemy.IsDead)
                                continue;

                            enemy.PullTowards(center, pullSpeed);
                        }
                    }

                    float waitTime = Mathf.Min(1f, remainingDuration);
                    yield return new WaitForSeconds(waitTime);
                    remainingDuration -= waitTime;
                }

            }
        }
    }
}