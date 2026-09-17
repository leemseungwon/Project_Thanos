using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Enemy.SlimeEnemies.Components
{
    [RequireComponent(typeof(Collider2D))]
    public class SlimeWhirlwindHitboxComponent
        : MonoBehaviour, IEntityComponent
    {
        [SerializeField]
        private float damageInterval = 0.25f;

        private SlimeEnemy _slime;
        private SlimeCombatComponent _combat;

        private PlayerController _player;

        private float _damageTimer;

        public void Initialize(BaseEntity owner)
        {
            _slime = owner as SlimeEnemy;

            if (_slime != null)
                _combat = _slime.SlimeCombat;
        }

        private void Update()
        {
            if (_player == null)
                return;

            _damageTimer -= Time.deltaTime;

            if (_damageTimer > 0f)
                return;

            _damageTimer = damageInterval;

            DealDamage();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player =
                other.GetComponent<PlayerController>();

            if (player == null)
                return;

            _player = player;
            _damageTimer = 0f;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerController player =
                other.GetComponent<PlayerController>();

            if (player == _player)
                _player = null;
        }

        private void DealDamage()
        {
            if (_slime == null ||
                _combat == null ||
                _slime.IsDead ||
                _player == null ||
                _player.IsStunned)
            {
                return;
            }

            AttackData attackData =
                _combat.WhirlwindAttackData;

            if (attackData == null)
                return;

            Vector2 direction =
                (_player.transform.position -
                 _slime.transform.position).normalized;

            int finalDamage =
                attackData.attackDamage +
                _slime.CurrentAttackDamage;

            _player.TakeDamage(
                finalDamage,
                direction,
                attackData.knockbackPower,
                attackData.stunDuration,
                _slime);
        }

        public void Reset()
        {
            _player = null;
            _damageTimer = 0f;
        }
    }
}