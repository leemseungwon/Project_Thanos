
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.Environment.Stone_Ability;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "MindStone", menuName = "So/Data/Stone Data/Stones/Mind Stone", order = 0)]
    public class MindStone : BaseStone
    {
        [SerializeField] private string mindAreaName = "StoneAbility_StoneArena";
        [SerializeField] private float radius = 2f;
        [SerializeField] private float duration = 5f;

        public float Radius => radius;
        public float Duration => duration;

        public MindStoneArea CurrentArena { get; private set; }
        
        public override void Use(PlayerController owner)
        {
            Vector2 position = owner.PlayerCamera.ScreenToWorldPoint(
                owner.Input.MousePosition);

            if (SystemHandler.Instance.GetSystem(out PoolSystem poolSystem))
            {
                if (poolSystem.SpawnPool(mindAreaName, out MindStoneArea arena))
                {
                    arena.SetInitialize(owner, position, radius, duration);
                    CurrentArena = arena;
                }
            }
        }
    }
}
