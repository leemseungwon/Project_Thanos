using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Enemy;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System.PoolSystems;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class NormalRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Normal;

        [Header("Normal Room Settings")]
        [SerializeField] private int enemyCount = 3;
        [SerializeField] private string enemyPoolName = "Enemy_Golem";
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private UI.Rooms.RewardSelectionUI rewardSelectionUI;
        [SerializeField] private List<BaseStoneUpgrade> rewardUpgrades = new();

        private readonly List<BaseEnemy> _spawnedEnemies = new();
        private int _aliveEnemiesCount;

        public override void Initialize()
        {
            base.Initialize();

            if (rewardSelectionUI != null)
                rewardSelectionUI.gameObject.SetActive(false);
        }

        public override void Spawn()
        {
            base.Spawn();
            _spawnedEnemies.Clear();
            _aliveEnemiesCount = 0;
            
            if (rewardSelectionUI != null)
                rewardSelectionUI.gameObject.SetActive(false);

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
                    enemy.OnDeathEvent += () => OnEnemyDeath(enemy);
                }
            }

            if (_aliveEnemiesCount == 0)
            {
                OnAllEnemiesDefeated();
            }
        }

        private void OnEnemyDeath(BaseEnemy enemy)
        {
            _aliveEnemiesCount--;
            if (_aliveEnemiesCount <= 0)
            {
                OnAllEnemiesDefeated();
            }
        }

        public void OnAllEnemiesDefeated()
        {
            ShowRewardSelection();
        }

        private void ShowRewardSelection()
        {
            if (rewardSelectionUI != null)
            {
                rewardSelectionUI.Show(this);
            }
            else
            {
                ApplyDefaultReward();
                ClearRoom();
            }
        }

        public void OnRewardSelected(int index)
        {
            PlayerController player = GetPlayer();
            if (player != null)
            {
                switch (index)
                {
                    case 0:
                        player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 5);
                        break;
                    case 1:
                        player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 20);
                        player.Heal(20);
                        break;
                    case 2:
                        if (rewardUpgrades.Count > 0 && player.GetCompo(out PlayerStoneComponent stoneComponent))
                        {
                            BaseStoneUpgrade upgrade = rewardUpgrades[Random.Range(0, rewardUpgrades.Count)];
                            if (upgrade != null)
                                stoneComponent.AddStoneUpgrade(upgrade);
                        }
                        else
                        {
                            player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Add, 0.5f);
                        }
                        break;
                    default:
                        player.Heal(30);
                        break;
                }
            }

            ClearRoom();

            if (rewardSelectionUI != null)
                rewardSelectionUI.gameObject.SetActive(false);
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
                    if (enemy != null && enemy.gameObject.activeSelf)
                    {
                        poolSystem.DespawnPool(enemyPoolName, enemy);
                    }
                }
            }
            _spawnedEnemies.Clear();
            _aliveEnemiesCount = 0;
        }
    }
}