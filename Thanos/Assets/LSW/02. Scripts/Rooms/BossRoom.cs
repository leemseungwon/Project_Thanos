using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.UI.Rooms.Reward_Selections;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class BossRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Boss;

        [Header("Boss Room Settings")]
        [SerializeField] private string bossPoolName;
        [SerializeField] private Transform bossSpawnPoint;
        [SerializeField] private float bossHpMultiplier = 4.0f;
        [SerializeField] private float bossDamageMultiplier = 2.0f;
        [SerializeField] private float bossScaleMultiplier = 1.6f;

        private BaseEnemy _spawnedBoss;
        
        public override void StartRooms()
        {
            SpawnBoss();
        }

        private void SpawnBoss()
        {
            Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position;

            PoolSystem poolSystem = GetPoolSystem();
            if (poolSystem != null && poolSystem.SpawnPool(bossPoolName, out BaseEnemy boss))
            {
                _spawnedBoss = boss;
                _spawnedBoss.transform.position = spawnPos;
                SetupBossStats(_spawnedBoss);
                SpawnedPoolObjects.Add(_spawnedBoss);
            }
            else
            {
                OnBossDefeated();
            }
        }

        private void SetupBossStats(BaseEnemy boss)
        {
            if (boss == null) return;

            boss.transform.localScale = Vector3.one * bossScaleMultiplier;
            boss.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Multiply, bossHpMultiplier);
            boss.Heal(boss.MaxHp);
            boss.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Multiply, bossDamageMultiplier);

            boss.OnDeathEvent -= OnBossDefeated;
            boss.OnDeathEvent += OnBossDefeated;
        }

        public void OnBossDefeated()
        {
            if (IsCleared)
                return;

            PlayerController player = GetPlayer();
            if (player == null)
            {
                ClearRoom();
                return;
            }

            RewardSelectionUI selectionUI = GetInGameUI()?.RewardSelectionUI;

            if (selectionUI != null)
            {
                Action showUpgradeChoices = () =>
                {
                    player.EnableInput(false);
                    selectionUI.ShowRandomUpgrades(upgrade =>
                    {
                        if (upgrade != null &&
                            player.GetCompo(out PlayerStoneComponent stoneComponent))
                        {
                            stoneComponent.AddStoneUpgrade(upgrade);
                        }
                        else if (upgrade == null)
                        {
                            ApplyBossReward(player, 0, new List<BaseStoneUpgrade>());
                            return;
                        }

                        player.EnableInput(true);
                        ClearRoom();
                    });
                };

                if (DropRewardPickup(showUpgradeChoices))
                    return;

                showUpgradeChoices();
                return;
            }

            ApplyBossReward(player, 0, new List<BaseStoneUpgrade>());
        }

        private void ApplyBossReward(PlayerController player, int index,
            List<BaseStoneUpgrade> availableUpgrades)
        {
            if (index == 0)
                player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 25);
            else if (index == 1)
            {
                player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 100);
                player.Heal(player.MaxHp);
            }
            else if (index == 2)
                player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Add, 2f);
            else
            {
                int upgradeIndex = index - 3;
                if (upgradeIndex >= 0 && upgradeIndex < availableUpgrades.Count &&
                    player.GetCompo(out PlayerStoneComponent stoneComponent))
                {
                    stoneComponent.AddStoneUpgrade(availableUpgrades[upgradeIndex]);
                }
            }

            player.EnableInput(true);
            ClearRoom();
        }
        public override void Despawn()
        {
            base.Despawn();
            if (_spawnedBoss != null)
            {
                _spawnedBoss.OnDeathEvent -= OnBossDefeated;
                _spawnedBoss.transform.localScale = Vector3.one;
                PoolSystem poolSystem = GetPoolSystem();
                if (poolSystem != null)
                {
                    poolSystem.DespawnPool(bossPoolName, _spawnedBoss);
                }
                _spawnedBoss = null;
            }
        }
    }
}
