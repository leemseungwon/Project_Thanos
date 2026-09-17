using LSW._02._Scripts.Entity.Enemy.SlimeEnemies;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies.Components
{
    [RequireComponent(typeof(Collider2D))]
    public class SlimeContactDamageComponent
        : MonoBehaviour, IEntityComponent
    {
        [SerializeField]
        private AttackData contactAttackData;

        private SlimeEnemy _slime;

        public void Initialize(BaseEntity owner)
        {
            _slime = owner as SlimeEnemy;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController player =
                collision.collider.GetComponent<PlayerController>();

            if (player == null)
                return;

            DealDamage(player);
        }

        private void DealDamage(PlayerController player)
        {
            if (_slime == null ||
                _slime.IsDead ||
                player == null ||
                player.IsStunned ||
                contactAttackData == null)
            {
                return;
            }

            Vector2 direction =
                (player.transform.position -
                 _slime.transform.position).normalized;

            int finalDamage =
                contactAttackData.attackDamage +
                _slime.CurrentAttackDamage;

            player.TakeDamage(
                finalDamage,
                direction,
                contactAttackData.knockbackPower,
                contactAttackData.stunDuration,
                _slime);
        }

        public void Reset()
        {
        }
    }
}