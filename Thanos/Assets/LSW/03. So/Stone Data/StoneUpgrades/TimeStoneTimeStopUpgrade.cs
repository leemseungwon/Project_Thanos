using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Entity.Player.Component;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades
{
    [CreateAssetMenu(fileName = "Time Stone TimeStop Upgrade", 
        menuName = "So/Data/Stone Upgrade Data/Time Stone/TimeStop", order = 0)]
    public class TimeStoneTimeStopUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float duration = 5f;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            base.OnUse(player, stone);
            Activate();
        }

        public void Activate()
        {
            Debug.Log(
                $"Time Stop : {duration}s"
            );

            // TODO
            // 모든 적 이동 정지
            
            Debug.Log("Time Stop");
        }
    }
}