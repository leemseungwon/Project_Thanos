using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.UI.Rooms.Reward_Selections;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class EventRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Event;

        public override void StartRooms()
        {
            PlayerController player = GetPlayer();
            RewardSelectionUI ui = GetInGameUI()?.RewardSelectionUI;
            if (player == null || ui == null)
            {
                ClearRoom();
                return;
            }

            Action showEventChoices = () =>
            {
                player.EnableInput(false);
                ui.ShowChoices("A Strange Encounter", new List<string>
                {
                    "Accept the Wound\nTake 25 damage, increase attack damage by 10",
                    "Offer Currency\nSpend 20 currency, fully restore health",
                    "Unstable Upgrade\nTake 20 damage, choose a stone upgrade",
                    "Walk Away\nLeave without risk or reward"
                }, index => ResolveEvent(index, player));
            };

            if (!DropRewardPickup(showEventChoices))
                showEventChoices();
        }

        private void ResolveEvent(int index, PlayerController player)
        {
            switch (index)
            {
                case 0:
                    player.TakeDamage(25, Vector2.zero, 0f, 0f, null);
                    if (player.IsDead) return;
                    player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 10);
                    Finish(player);
                    break;
                case 1:
                    if (!player.TrySpendCurrency(20))
                    {
                        ShowInsufficientCurrency(player);
                        return;
                    }
                    player.Heal(player.MaxHp);
                    Finish(player);
                    break;
                case 2:
                    player.TakeDamage(20, Vector2.zero, 0f, 0f, null);
                    if (player.IsDead) return;
                    RewardSelectionUI ui = GetInGameUI()?.RewardSelectionUI;
                    if (ui != null)
                    {
                        player.EnableInput(true);
                        Action showUpgradeChoices = () =>
                        {
                            player.EnableInput(false);
                            ui.ShowRandomUpgrades(upgrade =>
                            {
                                if (upgrade != null && player.GetCompo(out PlayerStoneComponent stones))
                                    stones.AddStoneUpgrade(upgrade);
                                Finish(player);
                            });
                        };

                        if (!DropRewardPickup(showUpgradeChoices))
                            showUpgradeChoices();
                    }
                    else
                    {
                        Finish(player);
                    }
                    break;
                default:
                    Finish(player);
                    break;
            }
        }

        private void ShowInsufficientCurrency(PlayerController player)
        {
            RewardSelectionUI ui = GetInGameUI()?.RewardSelectionUI;
            if (ui == null)
            {
                Finish(player);
                return;
            }

            ui.ShowChoices("Not Enough Currency", new List<string>
            {
                "Take 25 damage to gain 60 currency",
                "Give up the encounter and leave"
            }, index => ResolveEvent(index == 0 ? 0 : 3, player));
        }

        private void Finish(PlayerController player)
        {
            player.EnableInput(true);
            ClearRoom();
            GetInGameUI()?.RoomSelectUI?.Open();
        }
    }
}
