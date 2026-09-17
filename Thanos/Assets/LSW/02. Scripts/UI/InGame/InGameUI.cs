
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.InGame
{
    public class InGameUI : Graphic
    {
        public RoomSelectUI RoomSelectUI { get; private set;}
        public StoneUI StoneUI { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            RoomSelectUI = GetComponentInChildren<RoomSelectUI>();
            StoneUI = GetComponentInChildren<StoneUI>();
        }
    }
}