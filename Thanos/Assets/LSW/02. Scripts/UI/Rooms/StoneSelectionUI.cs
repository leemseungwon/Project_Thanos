using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.Rooms
{
    public class StoneSelectionUI : MonoBehaviour
    {
        [SerializeField] private StartRoom ownerRoom;
        [SerializeField] private List<Button> stoneButtons;
        
        public void Show(StartRoom room = null)
        {
            if (room != null)
                ownerRoom = room;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SelectStone(int stoneIndex)
        {
            // 인덱스를 StoneType으로 변환 (예시)
            StoneType selected = (StoneType)stoneIndex;
            if (ownerRoom != null)
                ownerRoom.OnStoneSelected(selected);
            Hide();
        }
    }
}
