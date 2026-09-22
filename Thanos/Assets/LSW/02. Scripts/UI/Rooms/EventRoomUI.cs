using LSW._02._Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LSW._02._Scripts.UI.Rooms
{
    public class EventRoomUI : MonoBehaviour
    {
        [SerializeField] private EventRoom ownerRoom;
        [SerializeField] private TextMeshProUGUI eventDescription;

        public void Show(string description, EventRoom room = null)
        {
            if (room != null)
                ownerRoom = room;
            if (eventDescription != null)
                eventDescription.text = description;
            
            gameObject.SetActive(true);
        }

        public void OnAcceptButtonClick()
        {
            if (ownerRoom != null)
                ownerRoom.AcceptRisk();
            gameObject.SetActive(false);
        }

        public void OnDeclineButtonClick()
        {
            if (ownerRoom != null)
                ownerRoom.DeclineRisk();
            gameObject.SetActive(false);
        }
    }
}
