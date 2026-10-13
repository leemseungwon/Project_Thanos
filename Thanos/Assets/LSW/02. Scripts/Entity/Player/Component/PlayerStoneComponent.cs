using System;
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._03._So.Stone_Data;
using LSW._03._So.Stone_Data.StoneUpgrades.Reality_Stone;
using LSW._03._So.Stone_Data.StoneUpgrades.Space_Stone;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerStoneComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Stone State")]
        [SerializeField] private CurrentStoneState stoneState;
        [SerializeField] private int maxStoneEquipCount = 6;
        [SerializeField, Min(0)] private int completeSetAttackBonus = 50;
        [SerializeField, Min(0.1f)] private float comboWindow = 2.5f;
        [SerializeField, Range(0f, 1f)] private float comboDamagePerSwitch = 0.25f;
        [SerializeField, Min(1)] private int maxComboSwitches = 3;

        private PlayerController _player;
        private PlayerMovementComponent _movementComponent;

        private BaseStone[] equipedStones;
        private int _currentIndex;
        private int _comboSwitchCount;
        private float _comboTimer;
        private readonly Dictionary<StoneType, StoneConditionRuntime> _conditionData = new();
        private bool _completeSetBonusApplied;
        
        public int CurrentIndex => _currentIndex;

        public BaseStone CurrentStone
        {
            get
            {
                if (equipedStones == null)
                    return null;

                if (_currentIndex < 0 ||
                    _currentIndex >= equipedStones.Length)
                    return null;

                return equipedStones[_currentIndex];
            }
        }
        
        public int RealityShieldUses => _realityShieldUses;

        public event Action<BaseStone, BaseStone> OnStoneChanged;
        public event Action<BaseStone, float, bool> OnUpdateStoneUsable;
        public event Action<BaseStone, BaseStone> OnStoneCombo;
        public event Action OnCompleteSetCollected;
        
        private int _realityShieldUses = 1;

        public void Initialize(BaseEntity owner)
        {
            _player = owner as PlayerController;
            if (_player == null)
                return;

            _movementComponent = _player.GetComponent<PlayerMovementComponent>();
            equipedStones = new BaseStone[maxStoneEquipCount];

            _currentIndex = 0;
            _conditionData.Clear();
            _completeSetBonusApplied = false;
        }

        private void Update()
        {
            if (_player == null)
                return;

            UpdateConditions();
            if (_comboTimer > 0f)
            {
                _comboTimer -= Time.deltaTime;
                if (_comboTimer <= 0f)
                    _comboSwitchCount = 0;
            }
        }

        private void UpdateConditions()
        {
            UpdateTimeCondition();
            UpdateMindCondition();
        }

        #region Condition Runtime

        private StoneConditionRuntime GetConditionData(StoneType type)
        {
            if (_conditionData.TryGetValue(type, out StoneConditionRuntime data))
            {
                return data;
            }

            data = new StoneConditionRuntime();
            _conditionData.Add(type, data);
            return data;
        }

        #endregion

        #region Time Condition

        private void UpdateTimeCondition()
        {
            if (!stoneState.HasStone(StoneType.Time))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Time);
            if (data.BlockTimer > 0f)
            {
                data.BlockTimer -= Time.deltaTime;
                return;
            }

            data.Timer += Time.deltaTime;

            if (data.Timer < 15f)
            {
                NotifyStoneUsable(StoneType.Time);
                return;
            }

            data.Timer -= 15f;
            data.Stack++;
            data.Charge++;

            stoneState.UpdateCanUseStone(StoneType.Time, true);
            NotifyStoneUsable(StoneType.Time);
        }

        #endregion

        #region Mind Condition

        private void UpdateMindCondition()
        {
            if (!stoneState.HasStone(StoneType.Mind))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Mind);

            if (data.BlockTimer > 0f)
            {
                data.BlockTimer -= Time.deltaTime;
                return;
            }

            data.Timer += Time.deltaTime;

            if (data.Timer < 10f)
            {
                NotifyStoneUsable(StoneType.Mind);
                return;
            }

            data.Timer -= 10f;
            data.Stack++;
            data.Charge++;

            stoneState.UpdateCanUseStone(StoneType.Mind, true);
            NotifyStoneUsable(StoneType.Mind);
        }

        #endregion

        #region Player Events

        public void OnPlayerDamaged()
        {
            UpdateTimeDamageCondition();
            UpdateRealityDamageCondition();
        }

        private void UpdateTimeDamageCondition()
        {
            if (!stoneState.HasStone(StoneType.Time))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Time);

            //data.Timer = 0f;
            data.BlockTimer = 3f;
            NotifyStoneUsable(StoneType.Time);
        }

        private void UpdateRealityDamageCondition()
        {
            if (!stoneState.HasStone(StoneType.Reality))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Reality);
            data.Counter++;

            if (data.Counter < 5)
            {
                NotifyStoneUsable(StoneType.Reality);
                return;
            }

            data.Counter = 0;
            data.Stack++;
            data.Charge = Mathf.Min(data.Charge + 1, GetMaximumCharges(StoneType.Reality));

            stoneState.UpdateCanUseStone(StoneType.Reality, true);
            NotifyStoneUsable(StoneType.Reality);
        }

        public void OnPlayerDash()
        {
            if (!stoneState.HasStone(StoneType.Space))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Space);
            if (data.Charge > 0)
                return;

            data.Counter++;
            if (data.Counter < 5)
            {
                NotifyStoneUsable(StoneType.Space);
                return;
            }

            data.Counter = 0;
            data.Stack++;
            data.Charge = GetMaximumCharges(StoneType.Space);

            stoneState.UpdateCanUseStone(StoneType.Space, true);
            NotifyStoneUsable(StoneType.Space);
        }

        public void OnPlayerAttack()
        {
            if (!stoneState.HasStone(StoneType.Mind))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Mind);
            data.Timer = 0f;
            data.BlockTimer = 5f;
            NotifyStoneUsable(StoneType.Mind);
        }

        public void OnEnemyKilled()
        {
            if (!stoneState.HasStone(StoneType.Soul))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Soul);
            data.Counter++;

            if (data.Counter < 3)
            {
                NotifyStoneUsable(StoneType.Soul);
                return;
            }

            data.Counter = 0;
            data.Stack++;
            data.Charge++;

            stoneState.UpdateCanUseStone(StoneType.Soul, true);
            NotifyStoneUsable(StoneType.Soul);
        }

        public void OnEnemyHit()
        {
            if (!stoneState.HasStone(StoneType.Power))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Power);

            data.Counter++;
            if (data.Counter < 7)
            {
                NotifyStoneUsable(StoneType.Power);
                return;
            }

            data.Counter = 0;
            data.Stack++;
            data.Charge++;

            stoneState.UpdateCanUseStone(StoneType.Power, true);

            NotifyStoneUsable(StoneType.Power);
        }

        #endregion

        #region Stone Use

        public void UseStone()
        {
            if (CurrentStone == null)
                return;

            StoneType type = CurrentStone.StoneType;

            if (!stoneState.CanUseStone(type))
                return;

            stoneState.UseStone(type, _player);
            ConsumeCharge(type);
            NotifyStoneUsable(type);
        }

        private void ConsumeCharge(StoneType type)
        {
            StoneConditionRuntime data = GetConditionData(type);

            switch (type)
            {
                case StoneType.Space:
                case StoneType.Reality:
                    data.Charge--;
                    data.Charge = Mathf.Max(data.Charge, 0);
                    stoneState.UpdateCanUseStone(type, data.Charge > 0);
                    break;

                default:
                    data.Charge = 0;
                    stoneState.UpdateCanUseStone(type, false);
                    break;
            }
        }

        private int GetMaximumCharges(StoneType type)
        {
            return type switch
            {
                StoneType.Space => stoneState.HasStoneUpgrade<SpaceUsableTwiceUpgrade>(type) ? 2 : 1,
                StoneType.Reality => stoneState.HasStoneUpgrade<RealityUsableTwiceUpgrade>(type) ? 2 : 1,
                _ => 1
            };
        }

        #endregion

        #region Time Revive

        public void EnableTimeRevive()
        {
            if (!stoneState.HasStone(StoneType.Time))
                return;

            StoneConditionRuntime data = GetConditionData(StoneType.Time);
            data.ReviveAvailable = true;
        }

        public bool TryUseTimeRevive()
        {
            if (!stoneState.HasStone(StoneType.Time))
                return false;

            StoneConditionRuntime data = GetConditionData(StoneType.Time);

            if (!data.ReviveAvailable)
                return false;

            data.ReviveAvailable = false;
            return true;
        }

        #endregion

        #region Stone

        public void GetStone(BaseStone stone)
        {
            if (stone == null)
                return;

            StoneType type = stone.StoneType;
            if (stoneState.HasStone(type))
                return;

            stoneState.AddStone(type, stone);

            for (int i = 0; i < equipedStones.Length; i++)
            {
                if (equipedStones[i] != null)
                    continue;

                EquipStone(i, stone);
                break;
            }

            if (!_completeSetBonusApplied && HasCompleteSet)
            {
                _completeSetBonusApplied = true;
                _player.ChangeEntityStat(EntityStatType.AttackDamage, MathOperation.Add,
                    completeSetAttackBonus);
                OnCompleteSetCollected?.Invoke();
            }
        }

        public void EquipStone(int index, BaseStone stone)
        {
            if (stone == null)
                return;

            if (equipedStones == null || index < 0 || index >= equipedStones.Length)
                return;

            BaseStone previousStone = equipedStones[index];
            bool isCurrentSlot = index == _currentIndex;

            if (isCurrentSlot && previousStone != null)
            {
                previousStone.OnSwitchExit(_player);
            }

            equipedStones[index] = stone;
            if (isCurrentSlot)
            {
                stone.OnSwitchEnter(_player);
                OnStoneChanged?.Invoke(previousStone, stone);
                NotifyStoneUsable(stone.StoneType);
            }
        }

        public void RefreshCurrentStoneUI()
        {
            if (CurrentStone == null)
                return;

            OnStoneChanged?.Invoke(null, CurrentStone);
            NotifyStoneUsable(CurrentStone.StoneType);
        }

        public void AddStoneUpgrade(BaseStoneUpgrade stoneUpgrade)
        {
            if (stoneUpgrade == null)
                return;

            stoneState.AddStoneUpgrade(stoneUpgrade.StoneType, stoneUpgrade, _player);
        }

        public bool HasStone(StoneType stoneType)
        {
            return stoneState.HasStone(stoneType);
        }
        
        public bool HasUpgrade(StoneType upgradeStoneType, BaseStoneUpgrade upgrade, Type type)
        {
            if (upgrade == null || type == null ||
                !typeof(BaseStoneUpgrade).IsAssignableFrom(type) ||
                !HasStone(upgradeStoneType))
                return false;
            
            return stoneState.HasStoneUpgrade(upgradeStoneType, type);
        }

        public bool HasCompleteSet
        {
            get
            {
                foreach (StoneType type in Enum.GetValues(typeof(StoneType)))
                    if (!stoneState.HasStone(type)) return false;
                return true;
            }
        }

        public int ApplyComboDamageBonus(int damage)
        {
            if (_comboSwitchCount <= 0 || _comboTimer <= 0f)
                return damage;

            int result = Mathf.RoundToInt(damage * (1f + _comboSwitchCount * comboDamagePerSwitch));
            _comboSwitchCount = 0;
            _comboTimer = 0f;
            return result;
        }

        #endregion

        #region Switch

        public void SwitchNext()
        {
            if (equipedStones == null)
                return;

            for (int i = 1; i <= equipedStones.Length; i++)
            {
                int index = (_currentIndex + i) % equipedStones.Length;
                if (equipedStones[index] == null)
                    continue;

                Switch(index);
                return;
            }
        }

        public void SwitchPrevious()
        {
            if (equipedStones == null)
                return;

            for (int i = 1; i <= equipedStones.Length;i++)
            {
                int index = (_currentIndex - i + equipedStones.Length) %
                            equipedStones.Length;
                if (equipedStones[index] == null)
                    continue;

                Switch(index);
                return;
            }
        }

        public void Switch(int index)
        {
            if (index < 0 || index >= equipedStones.Length)
                return;

            if (index == _currentIndex)
                return;

            BaseStone previousStone = CurrentStone;
            BaseStone currentStone = equipedStones[index];

            if (currentStone == null)
                return;

            previousStone?.OnSwitchExit(_player);
            
            _currentIndex = index;
            currentStone.OnSwitchEnter(_player);
            
            previousStone?.OnCombo(_player, currentStone);
            OnStoneChanged?.Invoke(previousStone, currentStone);
            OnStoneCombo?.Invoke(previousStone, currentStone);

            if (previousStone != null)
            {
                _comboSwitchCount = _comboTimer > 0f
                    ? Mathf.Min(_comboSwitchCount + 1, maxComboSwitches)
                    : 1;
                _comboTimer = comboWindow;
            }

            NotifyStoneUsable(currentStone.StoneType);
        }

        #endregion

        #region UI

        private void NotifyStoneUsable(StoneType type)
        {
            if (CurrentStone == null)
                return;

            if (CurrentStone.StoneType != type)
                return;

            OnUpdateStoneUsable?.Invoke(CurrentStone, 
                GetStoneUsablePercent(type), stoneState.CanUseStone(type));
        }

        private float GetStoneUsablePercent(StoneType type)
        {
            if (!_conditionData.TryGetValue(type, out StoneConditionRuntime data))
                return stoneState.CanUseStone(type) ? 1f : 0f;

            float percent = type switch
            {
                StoneType.Time => data.Charge > 0 ? 1f : data.Timer / 15f,
                StoneType.Mind => data.Charge > 0 ? 1f : data.Timer / 10f,
                StoneType.Reality => (data.Charge + data.Counter / 5f) /
                                     GetMaximumCharges(StoneType.Reality),
                StoneType.Space => data.Charge > 0
                    ? data.Charge / (float)GetMaximumCharges(StoneType.Space)
                    : data.Counter / 5f,
                StoneType.Soul => data.Charge > 0 ? 1f : data.Counter / 3f,
                StoneType.Power => data.Charge > 0 ? 1f : data.Counter / 7f,
                _ => stoneState.CanUseStone(type) ? 1f : 0f
            };

            if (stoneState.CanUseStone(type) && data.Charge == 0)
                return 1f;

            return Mathf.Clamp01(percent);
        }

        #endregion

        #region Input

        private void Start()
        {
            if (_player == null)
                return;

            _player.Input.OnUseStonePressed += UseStone;
        }

        #endregion

        public void Reset()
        {
            _conditionData.Clear();
            _comboSwitchCount = 0;
            _comboTimer = 0f;
        }
    }
}
