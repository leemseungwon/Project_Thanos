using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.Environment.Interactable;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class RewardRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Reward;

        [Header("Reward Room Settings")]
        [SerializeField] private GameObject treasureChest;
        [SerializeField] private InteractionEventObject chestInteractable;
        [SerializeField] private int healAmount = 50;
        [SerializeField] private int bonusAttackDamage = 10;
        [SerializeField] private int bonusMaxHp = 20;
        [SerializeField] private List<BaseStoneUpgrade> chestUpgrades = new();

        private bool _chestOpened;

        public override void Initialize()
        {
            base.Initialize();
            _chestOpened = false;
            if (chestInteractable == null && treasureChest != null)
                chestInteractable = treasureChest.GetComponentInChildren<InteractionEventObject>(true);
        }

        public override void Spawn()
        {
            base.Spawn();
            _chestOpened = false;

            if (treasureChest != null)
                treasureChest.SetActive(true);

            if (chestInteractable == null && treasureChest != null)
                chestInteractable = treasureChest.GetComponentInChildren<InteractionEventObject>(true);

            if (chestInteractable != null)
            {
                chestInteractable.onInteract.RemoveListener(OnChestOpened);
                chestInteractable.onInteract.AddListener(OnChestOpened);
            }
        }

        public void OnChestOpened()
        {
            if (_chestOpened) return;
            _chestOpened = true;

            PlayerController player = GetPlayer();
            if (player != null)
            {
                player.Heal(healAmount);
                player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, bonusAttackDamage);
                player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, bonusMaxHp);

                if (chestUpgrades.Count > 0 && player.GetCompo(out PlayerStoneComponent stoneComponent))
                {
                    BaseStoneUpgrade upgrade = chestUpgrades[Random.Range(0, chestUpgrades.Count)];
                    if (upgrade != null)
                        stoneComponent.AddStoneUpgrade(upgrade);
                }
            }

            ClearRoom();

            if (treasureChest != null)
            {
                treasureChest.SetActive(false);
            }
        }
    }
}