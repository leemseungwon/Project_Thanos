using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.StoneUpgrades
{
    [CreateAssetMenu(fileName = "Space Stone Shockwave Upgrade", 
        menuName = "So/Data/Stone Upgrade Data/Space Stone/Shockwave", order = 0)]
    public class SpaceStoneShockwaveUpgrade : BaseStoneUpgrade
    {
        [SerializeField] private float damage = 7f;
        [SerializeField] private float radius = 2f;
        [SerializeField] private LayerMask targetLayer;

        public override void OnUse(PlayerController player, BaseStone stone)
        {
            base.OnUse(player, stone);
            CreateShockwave(player.transform.position);
        }

        public void CreateShockwave(Vector2 position)
        {
            Collider2D[] hits =
                Physics2D.OverlapCircleAll(position, radius, targetLayer);

            foreach (Collider2D hit in hits)
            {
                // TODO Damage
            }
            
            Debug.Log("Shockwave");
        }
    }
}