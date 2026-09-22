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
    public class EliteRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Elite;

        [Header("Elite Room Settings")]
        [SerializeField] private bool isBossType = true;
        [SerializeField] private int eliteCount = 1;
        [SerializeField] private string elitePoolName = "Enemy_Golem";
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float hpMultiplier = 2.0f;
        [SerializeField] private float damageMultiplier = 1.5f;
        [SerializeField] private float scaleMultiplier = 1.3f;
        [SerializeField] private UI.Rooms.RewardSelectionUI specialRewardUI;
        [SerializeField] private List<BaseStoneUpgrade> eliteUpgrades = new();

        private readonly List<BaseEnemy> _spawnedElites = new();
        private int _aliveEliteCount;

        public override void Initialize()
        {
            base.Initialize();

            if (specialRewardUI != null)
                specialRewardUI.gameObject.SetActive(false);
        }

        public override void Spawn()
        {
            base.Spawn();
            _spawnedElites.Clear();
            _aliveEliteCount = 0;

            if (specialRewardUI != null)
                specialRewardUI.gameObject.SetActive(false);

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
                if (poolSystem.SpawnPool(elitePoolName, out BaseEnemy enemy))
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
                    enemy.transform.localScale = Vector3.one * scaleMultiplier;

                    // ¿¤¸®Æ® ¸ó½ºÅÍ ½ºÅÈ °­È­
                    enemy.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Multiply, hpMultiplier);
                    enemy.InitStat();
                    enemy.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Multiply, damageMultiplier);

                    _spawnedElites.Add(enemy);
                    _aliveEliteCount++;
                    enemy.OnDeathEvent += () => OnEnemyDeath(enemy);
                }
            }

            if (_aliveEliteCount == 0)
            {
                OnEliteDefeated();
            }
        }

        private void OnEnemyDeath(BaseEnemy enemy)
        {
            _aliveEliteCount--;
            if (_aliveEliteCount <= 0)
            {
                OnEliteDefeated();
            }
        }

        public void OnEliteDefeated()
        {
            ShowSpecialReward();
        }

        private void ShowSpecialReward()
        {
            if (specialRewardUI != null)
            {
                specialRewardUI.Show(this);
            }
            else
            {
                ApplySpecialReward(0);
                ClearRoom();
            }
        }

        public void OnRewardSelected(int index = 0)
        {
            ApplySpecialReward(index);
            ClearRoom();

            if (specialRewardUI != null)
                specialRewardUI.gameObject.SetActive(false);
        }

        private void ApplySpecialReward(int index)
        {
            PlayerController player = GetPlayer();
            if (player != null)
            {
                switch (index)
                {
                    case 0:
                        player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 15);
                        break;
                    case 1:
                        player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 50);
                        player.Heal(player.MaxHp);
                        break;
                    case 2:
                        if (eliteUpgrades.Count > 0 && player.GetCompo(out PlayerStoneComponent stoneComponent))
                        {
                            BaseStoneUpgrade upgrade = eliteUpgrades[Random.Range(0, eliteUpgrades.Count)];
                            if (upgrade != null)
                                stoneComponent.AddStoneUpgrade(upgrade);
                        }
                        else
                        {
                            player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Add, 1.5f);
                        }
                        break;
                    default:
                        player.Heal(player.MaxHp);
                        break;
                }
            }
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
                        enemy.transform.localScale = Vector3.one;
                        poolSystem.DespawnPool(elitePoolName, enemy);
                    }
                }
            }
            _spawnedElites.Clear();
            _aliveEliteCount = 0;
        }
    }
}