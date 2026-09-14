using LSW._02._Scripts.Entity;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._03._So.Stone_Data
{
    [CreateAssetMenu(fileName = "FireStone", menuName = "So/Data/Stones/FireStone")]
    public class FireStone : BaseStone
    {
        [Header("Fire")]
        [SerializeField] private AttackData attackData;
        [SerializeField] private float range = 5f;

        public override void Use(PlayerController owner)
        {
            Vector2 direction = owner.LookDirection;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            RaycastHit2D hit = Physics2D.Raycast(
                owner.transform.position,
                direction,
                range,
                attackData.enemyLayer
            );

            Debug.DrawRay(
                owner.transform.position,
                direction * range,
                Color.red,
                0.2f
            );

            if (hit.collider == null)
                return;

            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(attackData,
                    attackData.attackDamage + owner.CurrentAttackDamage, owner);
            }
        }

        public override void OnSwitchEnter(PlayerController owner)
        {
            Debug.Log("FireStone Enter");
        }

        public override void OnSwitchExit(PlayerController owner)
        {
            Debug.Log("FireStone Exit");
        }

        public override void OnCombo(PlayerController player, BaseStone previousStone)
        {
            Debug.Log($"FireStone Combo ก็ {previousStone.StoneName}");
        }
    }
}