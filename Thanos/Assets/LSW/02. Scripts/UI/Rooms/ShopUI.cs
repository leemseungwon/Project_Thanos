using LSW._02._Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.Rooms
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private ShopRoom ownerRoom;
        
        public void Open(ShopRoom room = null)
        {
            if (room != null)
                ownerRoom = room;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (ownerRoom != null)
                ownerRoom.OnExitShop();
            gameObject.SetActive(false);
        }

        public void OnBuyButtonClick(int itemIndex)
        {
            // 예시: 비용은 고정값이거나 데이터에서 가져옴
            int cost = 100; 
            if (ownerRoom != null)
                ownerRoom.PurchaseItem(itemIndex, cost);
        }
        
        public void OnExitButtonClick()
        {
            Close();
        }
    }
}
