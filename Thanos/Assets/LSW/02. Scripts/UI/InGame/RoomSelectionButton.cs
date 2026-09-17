
using LSW._02._Scripts.Common;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.System.RoomSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.InGame
{
    public class RoomSelectionButton : MonoBehaviour, IPoolable
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI nameText;

        private RoomNode _room;
        private RoomSelectUI _roomSelectUI;

        public void SetData(
            RoomNode room,
            RoomSelectUI roomSelectUI,
            bool interactable)
        {
            _room = room;
            _roomSelectUI = roomSelectUI;
            
            nameText.SetText(room.Type.ToString());

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            
            button.interactable = interactable;
        }

        private void OnClick()
        {
            if (_room == null)
                return;

            _roomSelectUI.SelectRoom(_room);
        }

        public void Initialize()
        {
            button.interactable = false;
        }

        public void Spawn()
        {
            _room = null;
            _roomSelectUI = null;

            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }

        public void Despawn()
        {
            _room = null;
            _roomSelectUI = null;

            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }
}