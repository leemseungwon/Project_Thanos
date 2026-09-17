using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "SpaceStone", menuName = "So/Data/Stone Data/Space Stone", order = 0)]
    public class SpaceStone : BaseStone
    {
        [Header("Space")]
        public float requiredMoveTime = 30f;

        public override void Use(PlayerController player)
        {
            Vector3 previousPosition =
                player.transform.position;

            Vector3 targetPosition =
                GetMouseWorldPosition(player);

            player.transform.position = targetPosition;

            player.Movement.ResetMoveTime();

            Debug.Log(
                $"Space Teleport : " +
                $"{previousPosition} ¡æ {targetPosition}"
            );
        }

        private Vector3 GetMouseWorldPosition(
            PlayerController player)
        {
            Ray ray =
                player.PlayerCamera.ScreenPointToRay(
                    player.Input.MousePosition
                );

            Plane plane =
                new Plane(
                    Vector3.forward,
                    player.transform.position
                );

            if (!plane.Raycast(ray, out float distance))
                return player.transform.position;

            return ray.GetPoint(distance);
        }
    }
}