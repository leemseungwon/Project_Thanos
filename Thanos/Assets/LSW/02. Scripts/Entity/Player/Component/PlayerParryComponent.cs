using System;
using LSW._02._Scripts.Common;
using LSW._03._So.Attack_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerParryComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Parry")] 
        [SerializeField] private AttackData parryAttackData;
        [SerializeField] private float parryDuration = 0.25f;
        [SerializeField] private float parryCooldown = 1f;

        [SerializeField, Range(0f, 1f)]
        private float reflectRatio = 0.1f;
        
        private PlayerController _player;
        private PlayerAnimation _playerAnimation;
        private PlayerCombatComponent _combatComponent;
        private float _parryTimer;
        private float _cooldownTimer;

        public bool IsParrying { get; private set; }

        public bool CanParry => !IsParrying && _cooldownTimer <= 0f;

        public event Action OnParryStart;
        public event Action<bool> OnParryEnd;

        private bool _isParryingSuccess;
        private ParryData? _parryData;

        public void Initialize(BaseEntity owner)
        {
            _player = owner as PlayerController;
            if (_player != null)
            {
                _player.GetCompo(out _playerAnimation);
                _player.GetCompo(out _combatComponent);
                if (_playerAnimation != null)
                {
                    _playerAnimation.OnParryAttackCast += Parry;
                    _playerAnimation.OnReadyParryAnimationEnd += EndReadyParry;
                }
            }
        }

        private void Update()
        {
            UpdateCooldown();
            UpdateParry();
        }

        private void UpdateCooldown()
        {
            if (_cooldownTimer <= 0f)
                return;

            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer < 0f)
                _cooldownTimer = 0f;
        }

        private void UpdateParry()
        {
            if (!IsParrying)
                return;

            _parryTimer -= Time.deltaTime;

            if (_parryTimer <= 0f && parryAttackData != null)
            {
                EndReadyParry();
            }
        }

        public bool TryStartParry()
        {
            if (!CanParry)
                return false;

            IsParrying = true;
            _parryTimer = parryDuration;

            OnParryStart?.Invoke();
            
            return true;
        }

        public bool TryParry(BaseEntity attacker, int damage, float knockBackPower, float stunDuration)
        {
            if (!IsParrying || _parryData != null || parryAttackData == null)
                return false;

            int reflectDamage =
                Mathf.FloorToInt(damage * reflectRatio)
                + parryAttackData.attackDamage
                + _player.CurrentAttackDamage;

            if (attacker == null || reflectDamage <= 0)
                return false;

            _parryData = new ParryData(reflectDamage, knockBackPower, stunDuration);

            _isParryingSuccess = true;

            CompleteParry();

            return true;
        }

        public void Parry()
        {
            if(_parryData == null || _combatComponent == null)
                return;
            
            int damage = _parryData.Value.ReflectDamage;
            float knockBackPower = _parryData.Value.KnockbackPower;
            float stunDuration = _parryData.Value.StunDuration;
            
            _parryData = null;
            
            _combatComponent.Attack(damage, knockBackPower, stunDuration);
        }

        public void EndReadyParry()
        {
            if (!IsParrying)
                return;
            
            _cooldownTimer = parryCooldown;
            
            OnParryEnd?.Invoke(_isParryingSuccess);
            _isParryingSuccess = false;
            
            IsParrying = false;
            _parryTimer = 0f;
        }
        
        public void CompleteParry()
        {
            if (!IsParrying)
                return;

            _cooldownTimer = parryCooldown;

            IsParrying = false;
            _parryTimer = 0f;

            OnParryEnd?.Invoke(_isParryingSuccess);
        }
        
        public void Reset() { }

        private void OnDestroy()
        {
            if (_playerAnimation != null)
            {
                _playerAnimation.OnParryAttackCast -= Parry;
                _playerAnimation.OnReadyParryAnimationEnd -= EndReadyParry;
            }
        }
    }
}