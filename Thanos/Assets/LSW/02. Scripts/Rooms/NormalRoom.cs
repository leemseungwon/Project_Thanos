using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.StoneSystems;
using LSW._02._Scripts.UI.InGame;
using LSW._02._Scripts.UI.Rooms.Reward_Selections;
using LSW._03._So.Reward_Data;
using LSW._03._So.Stone_Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSW._02._Scripts.Rooms
{
    public class NormalRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Normal;

        [Header("Normal Room Settings")]
        [SerializeField] private int enemyCount = 3;
        [SerializeField, Range(0f, 1f)] private float rewardOrbDropChance = 0.5f;
        [SerializeField] private string[] enemyPoolNames;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private StatRewardSo statReward;

        private readonly List<BaseEnemy> _spawnedEnemies = new();
        private readonly Dictionary<BaseEnemy, EnemyPoolEntry> _enemyPoolEntries = new();
        private RewardSelectionUI rewardSelectionUI;
        private StoneSystem _stoneSystem;
        private int _aliveEnemiesCount;
        private readonly List<BaseUpgradeSo> _pendingRewardCards = new();

        private sealed class EnemyPoolEntry
        {
            public string PoolName;
            public Action DeathHandler;
            public bool IsAlive;
        }

        private RewardSelectionUI GetRewardSelectionUI()
        {
            if (rewardSelectionUI == null)
            {
                InGameUI inGameUI = GetInGameUI();
                if (inGameUI != null)
                    rewardSelectionUI = inGameUI.RewardSelectionUI;
            }
            return rewardSelectionUI;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Spawn()
        {
            base.Spawn();
            _spawnedEnemies.Clear();
            _enemyPoolEntries.Clear();
            _aliveEnemiesCount = 0;
        }

        public override void StartRooms()
        {
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            PoolSystem poolSystem = GetPoolSystem();
            if (poolSystem == null || enemyCount <= 0)
            {
                OnAllEnemiesDefeated();
                return;
            }

            for (int i = 0; i < enemyCount; i++)
            {
                string enemyPoolName = GetRandomEnemy();
                if (string.IsNullOrEmpty(enemyPoolName))
                    continue;

                if (poolSystem.SpawnPool(enemyPoolName, out BaseEnemy enemy))
                {
                    Vector3 spawnPos = transform.position;
                    if (spawnPoints != null && spawnPoints.Length > 0)
                    {
                        spawnPos = spawnPoints[i % spawnPoints.Length].position;
                    }
                    else
                    {
                        float angle = i * (Mathf.PI * 2f / enemyCount);
                        spawnPos += new Vector3(Mathf.Cos(angle) * 3f, Mathf.Sin(angle) * 3f, 0f);
                    }

                    enemy.transform.position = spawnPos;
                    _spawnedEnemies.Add(enemy);
                    _aliveEnemiesCount++;

                    EnemyPoolEntry entry = new()
                    {
                        PoolName = enemyPoolName,
                        IsAlive = true
                    };
                    entry.DeathHandler = () => OnEnemyDeath(enemy);
                    _enemyPoolEntries[enemy] = entry;
                    enemy.OnDeathEvent += entry.DeathHandler;

                    SpawnedPoolObjects.Add(enemy);
                }
            }

            if (_aliveEnemiesCount == 0)
            {
                OnAllEnemiesDefeated();
            }
        }

        private string GetRandomEnemy()
        {
            if (enemyPoolNames == null || enemyPoolNames.Length == 0)
                return string.Empty;

            return enemyPoolNames[Random.Range(0, enemyPoolNames.Length)];
        }

        private void OnEnemyDeath(BaseEnemy enemy)
        {
            if (enemy == null || !_enemyPoolEntries.TryGetValue(enemy, out EnemyPoolEntry entry) ||
                !entry.IsAlive)
                return;

            entry.IsAlive = false;
            enemy.OnDeathEvent -= entry.DeathHandler;
            _aliveEnemiesCount--;
            if (Random.value < rewardOrbDropChance)
                DropRewardPickup(ShowRewardSelection, lockedUntilRoomClear: true,
                    spawnPosition: enemy.transform.position);
            if (_aliveEnemiesCount <= 0)
            {
                OnAllEnemiesDefeated();
            }
        }
        public void OnAllEnemiesDefeated()
        {
            UnlockRewardPickups();
            ClearRoom();
            if (_spawnedEnemies.Count == 0 || !HasRewardPickups)
                ShowRewardSelection();
        }

        private void ShowRewardSelection()
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
                player.EnableInput(false);
                BuildRewardCards(player);
                ui.ShowRewardOptions("Room Clear Reward", _pendingRewardCards,
                    index => ChooseReward(index, player));

                return;
            }

            if (statReward != null)
                statReward.Apply(player);
            else
                player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 25);
            CompleteRewardSelection(player);
        }

        private void BuildRewardCards(PlayerController player)
        {
            _pendingRewardCards.Clear();
            if (_stoneSystem == null && SystemHandler.Instance != null)
                SystemHandler.Instance.GetSystem(out _stoneSystem);

            player.GetCompo(out PlayerStoneComponent stoneComponent);
            for (int i = 0; i < 3; i++)
            {
                float roll = Random.value;
                BaseUpgradeSo reward;
                if (roll < 0.60f)
                {
                    reward = statReward;
                }
                else if (roll < 0.85f)
                {
                    BaseStoneUpgrade upgrade = _stoneSystem != null
                        ? _stoneSystem.GetRandomStoneUpgrade(stoneComponent)
                        : null;
                    reward = upgrade != null ? upgrade : statReward;
                }
                else
                {
                    BaseStone stone = _stoneSystem != null
                        ? _stoneSystem.GetRandomUnownedStone(stoneComponent)
                        : null;
                    reward = stone != null ? stone : statReward;
                }
                if (reward != null)
                    _pendingRewardCards.Add(reward);
            }
        }

        private void ChooseReward(int index, PlayerController player)
        {
            if (index < 0 || index >= _pendingRewardCards.Count)
            {
                CompleteRewardSelection(player);
                return;
            }

            BaseUpgradeSo reward = _pendingRewardCards[index];
            if (reward != null && reward.Apply(player))
            {
                CompleteRewardSelection(player);
            }
            else
                CompleteRewardSelection(player);
        }

        public void OnRewardSelected(int index)
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

                player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Add, 0.5f);
            }
            else if (index == 0)
                player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 5);
            else if (index == 1)
            {
                player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 20);
                player.Heal(20);
            }
            else
                player.Heal(30);

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
            _pendingRewardCards.Clear();
            player.EnableInput(true);
            ClearRoom();
        }

        private void ApplyDefaultReward()
        {
            PlayerController player = GetPlayer();
            if (player != null)
            {
                player.Heal(20);
            }
        }

        public override void Despawn()
        {
            base.Despawn();
            PoolSystem poolSystem = GetPoolSystem();
            if (poolSystem != null)
            {
                foreach (BaseEnemy enemy in _spawnedEnemies)
                {
                    if (enemy == null || !_enemyPoolEntries.TryGetValue(enemy, out EnemyPoolEntry entry))
                        continue;

                    enemy.OnDeathEvent -= entry.DeathHandler;
                    if (enemy.gameObject.activeSelf)
                        poolSystem.DespawnPool(entry.PoolName, enemy);
                }
            }
            _spawnedEnemies.Clear();
            _enemyPoolEntries.Clear();
            _aliveEnemiesCount = 0;
        }
    }
}
