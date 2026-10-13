using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.Environment.Interactable;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.StoneSystems;
using LSW._02._Scripts.UI.Rooms.Reward_Selections;
using LSW._03._So.Reward_Data;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class RewardRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Reward;

        [Header("Reward Room Settings")]
        [SerializeField] private InteractionEventObject interactableChest;
        [SerializeField] private HealRewardSo healReward;
        [SerializeField] private StatRewardSo statReward;
        [SerializeField] private UpgradeChoiceRewardSo stoneUpgradeReward;

        private bool _chestOpened;

        public override void Initialize()
        {
            base.Initialize();
            _chestOpened = false;
        }

        public override void Spawn()
        {
            base.Spawn();
            _chestOpened = false;

            if (interactableChest != null)
                interactableChest.gameObject.SetActive(true);

            if (interactableChest != null)
            {
                interactableChest.onInteract.RemoveListener(OnChestOpened);
                interactableChest.onInteract.AddListener(OnChestOpened);
            }
        }

        public override void StartRooms() { }

        public override void OnReenterClearedRoom()
        {
            _chestOpened = true;
            if (interactableChest != null)
                interactableChest.gameObject.SetActive(false);
        }

        public void OnChestOpened()
        {
            if (_chestOpened)
                return;

            _chestOpened = true;

            PlayerController player = GetPlayer();
            if (player == null)
                return;

            RewardSelectionUI selectionUI = GetInGameUI()?.RewardSelectionUI;

            List<BaseUpgradeSo> rewards = GetChestRewards();
            if (selectionUI != null && rewards.Count > 0)
            {
                player.EnableInput(false);
                selectionUI.ShowRewardOptions("Safe Reward", rewards, SelectChestReward);
                return;
            }

            healReward?.Apply(player);
            CompleteChest(player);
        }

        private void SelectChestReward(int index)
        {
            PlayerController player = GetPlayer();
            if (player == null)
                return;

            List<BaseUpgradeSo> rewards = GetChestRewards();
            if (index < 0 || index >= rewards.Count)
            {
                CompleteChest(player);
                return;
            }

            BaseUpgradeSo reward = rewards[index];
            if (reward is UpgradeChoiceRewardSo)
            {
                RewardSelectionUI selectionUI = GetInGameUI()?.RewardSelectionUI;
                if (selectionUI == null)
                {
                    CompleteChest(player);
                    return;
                }

                player.EnableInput(false);
                selectionUI.ShowRandomUpgrades(upgrade =>
                {
                    upgrade?.Apply(player);
                    CompleteChest(player);
                });
                return;
            }

            reward.Apply(player);
            CompleteChest(player);
        }

        private List<BaseUpgradeSo> GetChestRewards()
        {
            List<BaseUpgradeSo> rewards = new();
            if (healReward != null)
                rewards.Add(healReward);
            if (statReward != null)
                rewards.Add(statReward);
            if (stoneUpgradeReward != null)
                rewards.Add(stoneUpgradeReward);
            return rewards;
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

        private void CompleteChest(PlayerController player)
        {
            player.EnableInput(true);
            ClearRoom();

            if (interactableChest != null)
            {
                interactableChest.gameObject.SetActive(false);
            }
        }
    }
}
