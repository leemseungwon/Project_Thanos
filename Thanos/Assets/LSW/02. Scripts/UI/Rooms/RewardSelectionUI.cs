using System.Collections.Generic;
using LSW._02._Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.Rooms
{
    public class RewardSelectionUI : MonoBehaviour
    {
        [SerializeField] private List<Button> rewardButtons;
        private NormalRoom _normalRoom;
        private EliteRoom _eliteRoom;

        public void Show(NormalRoom room)
        {
            _normalRoom = room;
            _eliteRoom = null;
            gameObject.SetActive(true);
        }

        public void Show(EliteRoom room)
        {
            _eliteRoom = room;
            _normalRoom = null;
            gameObject.SetActive(true);
        }

        public void SelectReward(int index)
        {
            if (_normalRoom != null)
                _normalRoom.OnRewardSelected(index);
            else if (_eliteRoom != null)
                _eliteRoom.OnRewardSelected(index);

            gameObject.SetActive(false);
        }
    }
}
