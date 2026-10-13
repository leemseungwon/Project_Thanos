using LSW._02._Scripts.Entity.Player;
using UnityEngine;

namespace LSW._03._So.Stone_Data.Stones
{
    [CreateAssetMenu(fileName = "SpaceStone", menuName = "So/Data/Stone Data/Stones/Space Stone", order = 0)]
    public class SpaceStone : BaseStone
    {
        [Header("Space Stone")]
        [SerializeField] private float maxTeleportDistance = 100f;
        [SerializeField] private LayerMask teleportBlockLayerMask;

        private Vector2 _lastTeleportOrigin;
        private Vector2 _lastTeleportDestination;

        public Vector2 LastTeleportOrigin => _lastTeleportOrigin;
        public Vector2 LastTeleportDestination => _lastTeleportDestination;
        public bool DidTeleportOnLastUse { get; private set; }

        public override void Use(PlayerController owner)
        {
            DidTeleportOnLastUse = false;
            if (owner == null || owner.PlayerCamera == null || owner.Input == null)
                return;

            Vector2 origin = owner.Rigidbody.position;

            Vector3 mouseWorld = owner.PlayerCamera.ScreenToWorldPoint(
                    new Vector3(owner.Input.MousePosition.x, owner.Input.MousePosition.y,
                        Mathf.Abs(owner.PlayerCamera.transform.position.z - owner.transform.position.z
                        )
                    )
                );

            Vector2 destination = mouseWorld;
            Vector2 offset = destination - origin;

            if (offset.magnitude > maxTeleportDistance)
            {
                destination = origin + offset.normalized * maxTeleportDistance;
            }

            if (IsDestinationBlocked(owner, origin, destination, teleportBlockLayerMask))
                return;

            _lastTeleportOrigin = origin;
            _lastTeleportDestination = destination;

            owner.Rigidbody.position = destination;
            DidTeleportOnLastUse = true;
        }

        private static bool IsDestinationBlocked(PlayerController owner, Vector2 origin,
            Vector2 destination, LayerMask teleportBlockLayerMask)
        {
            Vector2 positionOffset = destination - origin;
            Collider2D playerColliders = owner.GetComponent<Collider2D>();
            
            if (!playerColliders.enabled || playerColliders.isTrigger)
                return true;

            Bounds bounds = playerColliders.bounds;
            Vector2 targetCenter = (Vector2)bounds.center + positionOffset;
            Collider2D[] overlaps = Physics2D.OverlapBoxAll(
                targetCenter, bounds.size, 0f, teleportBlockLayerMask);

            foreach (Collider2D overlap in overlaps)
            {
                if (!overlap.isTrigger && overlap.attachedRigidbody != owner.Rigidbody)
                    return true;
            }

            return false;
        }
    }
}