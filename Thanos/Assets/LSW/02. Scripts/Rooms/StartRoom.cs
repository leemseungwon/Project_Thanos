
using System;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.UI.InGame;
using LSW._02._Scripts.UI.Rooms.Stone_Selections;

namespace LSW._02._Scripts.Rooms
{
    public class StartRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Start;
        
        private PlayerController player;
        private StoneSelectionUI stoneSelectionUI;
        
        private StoneSelectionUI GetStoneSelectionUI()
        {
            if (stoneSelectionUI == null)
            {
                InGameUI inGameUI = GetInGameUI();
                if (inGameUI != null)
                {
                    stoneSelectionUI = inGameUI.StoneSelectionUI;
                }
            }
            return stoneSelectionUI;
        }

        public override void StartRooms()
        {
            ShowStoneSelection();
        }

        private void ShowStoneSelection()
        {
            var ui = GetStoneSelectionUI();
            if (ui == null)
            {
                ClearRoom();
                return;
            }

            Action showStoneSelection = () =>
            {
                GetPlayer()?.EnableInput(false);
                ui.Show(ClearRoom);
            };

            if (!DropRewardPickup(showStoneSelection))
                showStoneSelection();
        }

        public override void ClearRoom()
        {
            base.ClearRoom();
            GetPlayer().EnableInput(true);
        }
    }
}
