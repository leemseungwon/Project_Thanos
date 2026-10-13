using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.System.StoneSystems;
using LSW._02._Scripts.UI.Rooms.Reward_Selections;
using LSW._03._So.Stone_Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSW._02._Scripts.Rooms
{
    public class EliteRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Elite;

        [Header("Elite Room Settings")]
        [SerializeField] private bool isBossType = true;
        [SerializeField] private int eliteCount = 1;
        [SerializeField, Range(0f, 1f)] private float rewardOrbDropChance = 0.5f;
        [SerializeField] private string[] elitePoolName = { "Enemy_Rat" };
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float hpMultiplier = 2.0f;
        [SerializeField] private float damageMultiplier = 1.5f;
        [SerializeField] private float scaleMultiplier = 1.3f;
        private readonly List<BaseEnemy> _spawnedElites = new();
        private readonly Dictionary<BaseEnemy, string> _elitePoolNames = new();
        private int _aliveEliteCount;

        private RewardSelectionUI GetRewardSelectionUI()
        {
            return GetInGameUI()?.RewardSelectionUI;
        }
        
        public override void Spawn()
        {
            base.Spawn();
            _spawnedElites.Clear();
            _elitePoolNames.Clear();
            _aliveEliteCount = 0;
        }

        public override void StartRooms()
        {
            SpawnElites();
        }
        
        private void SpawnElites()
        {
            PoolSystem poolSystem = GetPoolSystem();
            int countToSpawn = isBossType ? 1 : Mathf.Max(1, eliteCount);

            if (poolSystem == null)
            {
                OnEliteDefeated();
                return;
            }

            for (int i = 0; i < countToSpawn; i++)
            {
                if (poolSystem.SpawnPool(elitePoolName[i % elitePoolName.Length], out BaseEnemy enemy))
                {
                    Vector3 spawnPos = transform.position;
                    if (spawnPoints != null && spawnPoints.Length > 0)
                    {
                        spawnPos = spawnPoints[i % spawnPoints.Length].position;
                    }
                    else
                    {
                        spawnPos += new Vector3((i - countToSpawn / 2f) * 2f, 0f, 0f);
                    }

                    enemy.transform.position = spawnPos;
                    enemy.transform.localScale = enemy.InitialLocalScale * scaleMultiplier;

                    // ¿¤¸®Æ® ¸ó½ºÅÍ ½ºÅÈ °­È­
                    enemy.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Multiply, hpMultiplier);
                    enemy.Heal(enemy.MaxHp);
                    enemy.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Multiply, damageMultiplier);

                    _spawnedElites.Add(enemy);
                    _elitePoolNames[enemy] = elitePoolName[i % elitePoolName.Length];
                    _aliveEliteCount++;
                    enemy.OnDeathEvent += () => OnEnemyDeath(enemy);
                    
                    SpawnedPoolObjects.Add(enemy);
                }
            }

            if (_aliveEliteCount == 0)
            {
                OnEliteDefeated();
            }
        }

        private void OnEnemyDeath(BaseEnemy enemy)
        {
            if (Random.value < rewardOrbDropChance)
                DropRewardPickup(ShowSpecialReward, lockedUntilRoomClear: true,
                    spawnPosition: enemy.transform.position);
            _aliveEliteCount--;
            if (_aliveEliteCount <= 0)
            {
                OnEliteDefeated();
            }
        }

        public void OnEliteDefeated()
        {
            UnlockRewardPickups();
            ClearRoom();
            if (_spawnedElites.Count == 0 || !HasRewardPickups)
                ShowSpecialReward();
        }

        private void ShowSpecialReward()
        {
            PlayerController player = GetPlayer();
            RewardSelectionUI ui = GetRewardSelectionUI();

            if (player == null)
            {
                ClearRoom();
                return;
            }

            if (ui != null)
            {
                Action showUpgradeChoices = () =>
                {
                    player.EnableInput(false);
                    ui.ShowRandomUpgrades(upgrade =>
                    {
                        if (upgrade != null &&
                            player.GetCompo(out PlayerStoneComponent stoneComponent))
                        {
                            stoneComponent.AddStoneUpgrade(upgrade);
                        }

                        CompleteRewardSelection(player);
                    });
                };

                if (DropRewardPickup(showUpgradeChoices))
                    return;

                showUpgradeChoices();
                return;
            }

            player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 15);
            CompleteRewardSelection(player);
        }

        public void OnRewardSelected(int index = 0)
        {
            PlayerController player = GetPlayer();
            if (player == null)
                return;

            if (index == 2)
            {
                List<BaseStoneUpgrade> availableUpgrades = GetAvailableUpgrades(player);
                if (availableUpgrades.Count > 0)
                {
                    RewardSelectionUI ui = GetRewardSelectionUI();
                    if (ui != null)
                    {
                        player.EnableInput(true);
                        Action showUpgradeChoices = () =>
                        {
                            player.EnableInput(false);
                            ui.ShowRandomUpgrades(upgrade =>
                            {
                                if (upgrade != null &&
                                    player.GetCompo(out PlayerStoneComponent stoneComponent))
                                {
                                    stoneComponent.AddStoneUpgrade(upgrade);
                                }

                                CompleteRewardSelection(player);
                            });
                        };

                        if (!DropRewardPickup(showUpgradeChoices))
                            showUpgradeChoices();
                        return;
                    }
                }

                player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Add, 1.5f);
            }
            else if (index == 0)
                player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 15);
            else if (index == 1)
            {
                player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 50);
                player.Heal(player.MaxHp);
            }
            else
                player.Heal(player.MaxHp);

            CompleteRewardSelection(player);
        }

        private List<BaseStoneUpgrade> GetAvailableUpgrades(PlayerController player)
        {
            List<BaseStoneUpgrade> result = new();
            if (SystemHandler.Instance.GetSystem(out StoneSystem stoneSystem))
            {
                if (!player.GetCompo(out PlayerStoneComponent stoneComponent))
                    return result;

                BaseStoneUpgrade upgrade = stoneSystem.GetRandomStoneUpgrade(stoneComponent);
                if (upgrade != null && stoneComponent.HasStone(upgrade.StoneType))
                    result.Add(upgrade);
            }
            return result;
        }

        private static List<string> GetUpgradeLabels(List<BaseStoneUpgrade> upgrades)
        {
            List<string> labels = new();
            foreach (BaseStoneUpgrade upgrade in upgrades)
                labels.Add($"{upgrade.UpgradeName}\n{upgrade.Description}");
            return labels;
        }

        private void CompleteRewardSelection(PlayerController player)
        {
            player.EnableInput(true);
            ClearRoom();
        }
        public override void Despawn()
        {
            base.Despawn();
            PoolSystem poolSystem = GetPoolSystem();
            if (poolSystem != null)
            {
                foreach (BaseEnemy enemy in _spawnedElites)
                {
                    if (enemy != null && enemy.gameObject.activeSelf)
                    {
                        if (_elitePoolNames.TryGetValue(enemy, out string poolName))
                            poolSystem.DespawnPool(poolName, enemy);
                    }
                }
            }
            _spawnedElites.Clear();
            _elitePoolNames.Clear();
            _aliveEliteCount = 0;
        }
    }
}