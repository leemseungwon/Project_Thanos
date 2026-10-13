using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.UI.Rooms.Reward_Selections;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class ShopRoom : BaseRoom
    {
        [SerializeField, Min(0)] private int healCost = 20;
        [SerializeField, Min(0)] private int attackUpgradeCost = 30;
        [SerializeField, Min(0)] private int maxHealthUpgradeCost = 35;

        public override RoomType Type => RoomType.Shop;

        public override void StartRooms()
        {
            ShowShop();
        }

        private void ShowShop()
        {
            PlayerController player = GetPlayer();
            RewardSelectionUI ui = GetInGameUI()?.RewardSelectionUI;
            if (player == null || ui == null)
            {
                ClearRoom();
                return;
            }

            player.EnableInput(false);
            List<string> options = new()
            {
                $"Restore Health\nCost: {healCost} currency",
                $"Increase Attack by 5\nCost: {attackUpgradeCost} currency",
                $"Increase Max Health by 25\nCost: {maxHealthUpgradeCost} currency",
                "Leave Shop"
            };
            ui.ShowChoices($"Shop · Currency: {player.Currency}", options, index => Purchase(index, player));
        }

        private void Purchase(int index, PlayerController player)
        {
            if (index < 0 || index > 3)
            {
                player.EnableInput(true);
                ClearRoom();
                GetInGameUI()?.RoomSelectUI?.Open();
                return;
            }

            if (index == 3)
            {
                player.EnableInput(true);
                ClearRoom();
                GetInGameUI()?.RoomSelectUI?.Open();
                return;
            }

            int cost = index switch
            {
                0 => healCost,
                1 => attackUpgradeCost,
                2 => maxHealthUpgradeCost,
                _ => 0
            };

            if (!player.TrySpendCurrency(cost))
            {
                ShowShop();
                return;
            }

            switch (index)
            {
                case 0:
                    player.Heal(50);
                    break;
                case 1:
                    player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 5);
                    break;
                case 2:
                    player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 25);
                    break;
            }

            ShowShop();
        }
    }
}
