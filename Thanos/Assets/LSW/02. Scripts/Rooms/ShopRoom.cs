using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.Environment.Interactable;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class ShopRoom : BaseRoom
    {
        public override RoomType Type => RoomType.Shop;

        [Header("Shop Room Settings")]
        [SerializeField] private UI.Rooms.ShopUI shopUI;
        [SerializeField] private InteractionEventObject merchantInteractable;
        [SerializeField] private List<BaseStoneUpgrade> shopUpgrades = new();

        public override void Initialize()
        {
            base.Initialize();

            if (shopUI != null)
                shopUI.Close();
        }

        public override void Spawn()
        {
            base.Spawn();
            
            if (merchantInteractable != null)
            {
                merchantInteractable.onInteract.RemoveListener(OpenShop);
                merchantInteractable.onInteract.AddListener(OpenShop);
            }
            else
            {
                OpenShop();
            }

            // 상점방은 입장 시 포탈 이용 가능하도록 클리어 처리
            ClearRoom();
        }

        public void OpenShop()
        {
            if (shopUI == null)
                shopUI = FindFirstObjectByType<UI.Rooms.ShopUI>(FindObjectsInactive.Include);

            if (shopUI != null)
                shopUI.Open(this);
        }

        public void PurchaseItem(int itemIndex, int cost)
        {
            PlayerController player = GetPlayer();
            if (player != null)
            {
                switch (itemIndex)
                {
                    case 0: // 회복 포션
                        player.Heal(50);
                        break;
                    case 1: // 힘의 비약 (공격력 증가)
                        player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add, 10);
                        break;
                    case 2: // 활력의 반지 (최대 체력 증가 & 회복)
                        player.ChangeEntityStat(EntityStatType.MaxHp, MathOperation.Add, 30);
                        player.Heal(30);
                        break;
                    case 3: // 신속의 장화 (이동속도 증가)
                        player.ChangeEntityStat(EntityStatType.MoveSpeed, MathOperation.Add, 1f);
                        break;
                    case 4: // 스톤 업그레이드
                        if (shopUpgrades.Count > 0 && player.GetCompo(out PlayerStoneComponent stoneComponent))
                        {
                            BaseStoneUpgrade upgrade = shopUpgrades[Random.Range(0, shopUpgrades.Count)];
                            if (upgrade != null)
                                stoneComponent.AddStoneUpgrade(upgrade);
                        }
                        break;
                    default:
                        player.Heal(20);
                        break;
                }
            }

            ClearRoom();
        }

        public void OnExitShop()
        {
            ClearRoom();
        }
    }
}