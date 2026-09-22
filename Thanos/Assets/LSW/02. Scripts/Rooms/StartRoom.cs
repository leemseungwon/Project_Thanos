using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class StartRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Start;

        [Header("Start Room Settings")]
        [SerializeField] private UI.Rooms.StoneSelectionUI stoneSelectionUI;
        [SerializeField] private List<BaseStone> availableStones = new();

        public override void Initialize()
        {
            base.Initialize();

            if (stoneSelectionUI != null)
                stoneSelectionUI.Hide();
        }

        public override void Spawn()
        {
            base.Spawn();
            ShowStoneSelection();
        }

        private void ShowStoneSelection()
        {
            if (stoneSelectionUI != null)
            {
                stoneSelectionUI.Show(this);
            }
            else
            {
                ClearRoom();
            }
        }

        public void OnStoneSelected(StoneType selectedStone)
        {
            BaseStone targetStone = availableStones.Find(s => s != null && s.StoneType == selectedStone);
            if (targetStone != null)
            {
                OnStoneSelected(targetStone);
                return;
            }

            PlayerController player = GetPlayer();
            if (player != null && player.GetCompo(out PlayerStoneComponent stoneComponent))
            {
                if (targetStone != null)
                {
                    stoneComponent.GetStone(targetStone);
                }
            }

            ClearRoom();

            if (stoneSelectionUI != null)
                stoneSelectionUI.Hide();
        }

        public void OnStoneSelected(BaseStone selectedStone)
        {
            if (selectedStone != null)
            {
                PlayerController player = GetPlayer();
                if (player != null && player.GetCompo(out PlayerStoneComponent stoneComponent))
                {
                    stoneComponent.GetStone(selectedStone);
                }
            }

            ClearRoom();

            if (stoneSelectionUI != null)
                stoneSelectionUI.Hide();
        }
    }
}