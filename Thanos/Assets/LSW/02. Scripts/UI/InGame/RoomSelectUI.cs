using System;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.System.RoomSystems;
using UnityEngine;

namespace LSW._02._Scripts.UI.InGame
{
    public class RoomSelectUI : MonoBehaviour
    {
        [Header("Pool")]
        [SerializeField] private string roomButtonPool;

        [Header("UI")]
        [SerializeField] private RectTransform buttonParent;

        [Header("Hex Layout")]
        [SerializeField] private float hexSize = 100f;

        private PoolSystem _poolSystem;
        private RoomSystem _roomSystem;

        private readonly List<RoomSelectionButton> _spawnedButtons = new();

        private void Start()
        {
            if (!SystemHandler.Instance.GetSystem(out _roomSystem))
            {
                throw new Exception(
                    "RoomSystem is not initialized.");
            }

            if (!SystemHandler.Instance.GetSystem(out _poolSystem))
            {
                throw new Exception(
                    "PoolSystem is not initialized.");
            }
        }

        public void Open()
        {
            if (_roomSystem == null || _poolSystem == null)
                return;

            RoomNode currentRoom = _roomSystem.CurrentRoom;

            if (currentRoom == null)
                return;

            ClearButtons();

            // 현재 방에서 이동 가능한 방을 매번 새로 가져온다.
            IReadOnlyList<RoomNode> nextRooms =
                _roomSystem.GetNextRooms();

            HashSet<RoomNode> selectableRooms =
                new HashSet<RoomNode>();

            if (nextRooms != null)
            {
                foreach (RoomNode room in nextRooms)
                {
                    if (room != null)
                    {
                        selectableRooms.Add(room);
                    }
                }
            }

            // 전체 방 표시
            foreach (RoomNode room in _roomSystem.Rooms.Values)
            {
                if (!_poolSystem.SpawnPool(
                        roomButtonPool,
                        out RoomSelectionButton button))
                {
                    continue;
                }

                RectTransform rect =
                    button.GetComponent<RectTransform>();

                rect.SetParent(buttonParent, false);
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;

                // 실제 맵 좌표에 배치
                rect.anchoredPosition =
                    GetHexPosition(room.Coordinate);

                // 현재 방에서 갈 수 있는 방만 활성화
                bool interactable =
                    selectableRooms.Contains(room);

                button.SetData(
                    room,
                    this,
                    interactable);

                _spawnedButtons.Add(button);
            }

            // 현재 방이 항상 중앙에 오도록 맵 전체를 이동
            Vector2 currentPosition =
                GetHexPosition(currentRoom.Coordinate);

            buttonParent.anchoredPosition =
                -currentPosition;

            gameObject.SetActive(true);
        }

        private Vector2 GetHexPosition(
            Vector2Int coordinate)
        {
            float x = Mathf.Sqrt(3f) * hexSize * (coordinate.x + coordinate.y * 0.5f);

            float y = 1.5f * hexSize * coordinate.y;

            return new Vector2(x, y);
        }

        public void SelectRoom(RoomNode room)
        {
            if (room == null)
                return;
            
            IReadOnlyList<RoomNode> nextRooms = _roomSystem.GetNextRooms();

            if (nextRooms == null || !nextRooms.Contains(room))
            {
                return;
            }
            
            _roomSystem.SelectNextRoom(room);

            Close();
        }

        private void ClearButtons()
        {
            foreach (RoomSelectionButton button
                     in _spawnedButtons)
            {
                if (button == null)
                    continue;

                _poolSystem.DespawnPool(
                    roomButtonPool,
                    button);
            }

            _spawnedButtons.Clear();
        }

        public void Close()
        {
            ClearButtons();

            gameObject.SetActive(false);
        }
    }
}