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
    public class BossRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Boss;

        [Header("Boss Room Settings")]
        [SerializeField] private GameObject bossInstance;
        [SerializeField] private string bossPoolName = "Enemy_Golem";
        [SerializeField] private Transform bossSpawnPoint;
        [SerializeField] private float bossHpMultiplier = 4.0f;
        [SerializeField] private float bossDamageMultiplier = 2.0f;
        [SerializeField] private float bossScaleMultiplier = 1.6f;
        [SerializeField] private List<BaseStoneUpgrade> bossRewardUpgrades = new();

        private BaseEnemy _spawnedBoss;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Spawn()
        {
            base.Spawn();
            SpawnBoss();
        }

        private void SpawnBoss()
        {
            Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position;

            if (bossInstance != null)
            {
                bossInstance.SetActive(true);
                bossInstance.transform.position = spawnPos;
                if (bossInstance.TryGetComponent(out BaseEnemy enemy))
                {
                    _spawnedBoss = enemy;
                    SetupBossStats(_spawnedBoss);
                }
                return;
            }

            PoolSystem poolSystem = GetPoolSystem();
            if (poolSystem != null && poolSystem.SpawnPool(bossPoolName, out BaseEnemy boss))
            {
                _spawnedBoss = boss;
                _spawnedBoss.transform.position = spawnPos;
                SetupBossStats(_spawnedBoss);
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
            boss.InitStat();
            boss.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Multiply, bossDamageMultiplier);

            boss.OnDeathEvent += OnBossDefeated;
        }

        public void OnBossDefeated()
        {
            PlayerController player = GetPlayer();
            if (player != null)
            {
                // 보스 처치 보상 (모든 스탯 대폭 상승 및 완전 회복)
                player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 100);
                player.Heal(player.MaxHp);
                player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 25);
                player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Add, 2f);

                if (bossRewardUpgrades.Count > 0 && player.GetCompo(out PlayerStoneComponent stoneComponent))
                {
                    foreach (var upgrade in bossRewardUpgrades)
                    {
                        if (upgrade != null)
                            stoneComponent.AddStoneUpgrade(upgrade);
                    }
                }
            }

            ClearRoom();
        }

        public override void Despawn()
        {
            base.Despawn();
            if (_spawnedBoss != null)
            {
                _spawnedBoss.transform.localScale = Vector3.one;
                PoolSystem poolSystem = GetPoolSystem();
                if (poolSystem != null)
                {
                    poolSystem.DespawnPool(bossPoolName, _spawnedBoss);
                }
                _spawnedBoss = null;
            }

            if (bossInstance != null)
            {
                bossInstance.SetActive(false);
            }
        }
    }
}