using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.Environment.Interactable;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class EventRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Event;

        [Header("Event Room Settings")]
        [SerializeField] private UI.Rooms.EventRoomUI eventUI;
        [SerializeField] private InteractionEventObject eventInteractable;
        [SerializeField] private List<BaseStoneUpgrade> eventUpgrades = new();

        private int _currentEventIndex = 0;

        private static readonly string[] EventDescriptions =
        {
            "피의 제단: 현재 체력 25를 바쳐 강력한 힘(공격력 +15)을 얻으시겠습니까?",
            "신성한 샘: 이동속도 1을 희생하여 생명력(최대 체력 +40 & 완전 회복)을 얻으시겠습니까?",
            "혼돈의 고대석: 위험을 감수(체력 20 소모)하고 스톤 업그레이드를 획득하시겠습니까?"
        };

        public override void Initialize()
        {
            base.Initialize();

            if (eventUI != null)
                eventUI.gameObject.SetActive(false);
        }

        public override void Spawn()
        {
            base.Spawn();
            _currentEventIndex = Random.Range(0, EventDescriptions.Length);

            if (eventInteractable != null)
            {
                eventInteractable.onInteract.RemoveListener(ShowEvent);
                eventInteractable.onInteract.AddListener(ShowEvent);
            }
            else
            {
                ShowEvent();
            }
        }

        public void ShowEvent()
        {
            if (eventUI == null)
                eventUI = FindFirstObjectByType<UI.Rooms.EventRoomUI>(FindObjectsInactive.Include);

            if (eventUI != null)
            {
                string desc = EventDescriptions[_currentEventIndex];
                eventUI.Show(desc, this);
            }
            else
            {
                ClearRoom();
            }
        }

        public void AcceptRisk()
        {
            PlayerController player = GetPlayer();
            if (player != null)
            {
                switch (_currentEventIndex)
                {
                    case 0: // 피의 제단: 체력 희생 -> 공격력 대폭 증가
                        player.TakeDamage(25, Vector2.zero, 0f, 0f, null, null);
                        player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 15);
                        break;
                    case 1: // 신성한 샘: 이동속도 감소 -> 최대 체력 대폭 증가 및 풀 회복
                        player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Subtract, 1f);
                        player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 40);
                        player.Heal(player.MaxHp);
                        break;
                    case 2: // 혼돈의 고대석: 체력 소모 -> 스톤 업그레이드
                        player.TakeDamage(20, Vector2.zero, 0f, 0f, null, null);
                        if (eventUpgrades.Count > 0 && player.GetCompo(out PlayerStoneComponent stoneComponent))
                        {
                            BaseStoneUpgrade upgrade = eventUpgrades[Random.Range(0, eventUpgrades.Count)];
                            if (upgrade != null)
                                stoneComponent.AddStoneUpgrade(upgrade);
                        }
                        else
                        {
                            player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 10);
                        }
                        break;
                }
            }

            ClearRoom();
            CloseEvent();
        }

        public void DeclineRisk()
        {
            ClearRoom();
            CloseEvent();
        }

        private void CloseEvent()
        {
            if (eventUI != null)
                eventUI.gameObject.SetActive(false);
        }
    }
}