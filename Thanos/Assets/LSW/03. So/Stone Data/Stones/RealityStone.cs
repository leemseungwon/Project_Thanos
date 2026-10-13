
using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "RealityStone", menuName = "So/Data/Stone Data/Stones/Reality Stone", order = 0)]
    public class RealityStone : BaseStone
    {
        [Header("Reality Stone")]
        [SerializeField] private float reflectionMultiplier = 1f;

        public override void Use(PlayerController owner)
        {
            if (owner == null)
                return;

            if (!owner.GetCompo(out EntityBuffComponent buff))
                return;

            buff.SetShield(reflectionMultiplier: 1f,
                remainingUses: 1);
        }
    }
}