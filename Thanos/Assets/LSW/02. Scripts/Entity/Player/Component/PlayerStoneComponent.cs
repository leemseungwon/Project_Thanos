using System;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Scripts.Common;
using LSW._03._So.Stone_Data;
using LSW._03._So.Stone_Data.Stones;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerStoneComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Stones")]
        [SerializeField] private CurrentStoneState stoneState;
        [SerializeField] private BaseStone[] equipedStones;
        [SerializeField] private int maxStoneEquipCount = 6;

        [Header("Init Test")]
        [SerializeField] private List<BaseStone> testStone;
        [SerializeField] private List<BaseStoneUpgrade> testStoneUpgrade;
        
        private PlayerController _player;
        private PlayerMovementComponent _movementComponent;
        private int _currentIndex;
        private float _timeStoneCooldown = 0f;
        private int _parryCount = 0;

        public int CurrentIndex => _currentIndex;

        public BaseStone CurrentStone
        {
            get
            {
                if (equipedStones == null || equipedStones.Length == 0)
                    return null;

                return equipedStones[_currentIndex];
            }
        }

        public event Action<BaseStone, BaseStone> OnStoneChanged;
        public event Action<BaseStone, BaseStone> OnStoneCombo;

        public void Initialize(BaseEntity owner)
        {
            _player = owner as PlayerController;
            if (_player != null)
            {
                _player.GetCompo(out _movementComponent);
            }
            
            equipedStones = new BaseStone[maxStoneEquipCount];
            _currentIndex = 0;
        }
        

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.Input.OnUseStonePressed -= UseStone;
                _player.Parry.OnParryEnd -= HandleParryEnd;
            }
        }

        private void Start()
        {
            InitTestStone();
            
            _player.Input.OnUseStonePressed += UseStone;
            _player.Parry.OnParryEnd += HandleParryEnd;
        }

        private void InitTestStone()
        {
            foreach (BaseStone stone in testStone)
            {
                GetStone(stone);
            }
            foreach (BaseStoneUpgrade stoneUpgrade in testStoneUpgrade)
            {
                AddStoneUpgrade(stoneUpgrade);
            }
        }

        public void GetStone(BaseStone stone)
        {
            stoneState.AddStone(stone.StoneType, stone);
            if (equipedStones.Count(s => s != null) < maxStoneEquipCount)
            {
                EquipStone(stone, GetEmptyStone());
            }
        }

        private int GetEmptyStone()
        {
            if (equipedStones == null)
                return -1;
            
            for (int i = 0; i < maxStoneEquipCount; i++)
            {
                if (equipedStones[i] == null)
                    return i;
            }
            return -1;
        }

        public void EquipStone(BaseStone stone, int changedIndex)
        {
            if(changedIndex == -1)
                return;
            
            if(equipedStones[changedIndex] != null)
                UnEquipStone(equipedStones[changedIndex], changedIndex);

            if (stone is TimeStone timestone && timestone != null)
                _timeStoneCooldown = timestone.Cooldown;
            
            equipedStones[changedIndex] = stone;
            OnStoneChanged?.Invoke(null, CurrentStone);
        }
        
        public void UnEquipStone(BaseStone stone, int index)
        {
            equipedStones[index] = null;
        }

        public void AddStoneUpgrade(BaseStoneUpgrade stoneUpgrade)
        {
            stoneState.AddStoneUpgrade(stoneUpgrade.StoneType, stoneUpgrade);
        }

        private void UseStone()
        {
            if(CurrentStone == null || !stoneState.CanUseStone(CurrentStone.StoneType))
                return;
            stoneState.UseStone(CurrentStone.StoneType, _player);
            stoneState.UpdateCanUseStone(CurrentStone.StoneType, false);
        }

        private void Update()
        {
            UpdateCanUseStone();
        }

        private void UpdateCanUseStone()
        {
            foreach (BaseStone stone in equipedStones)
            {
                if(stone == null)
                    continue;
                switch (stone.StoneType)
                {
                    case StoneType.Space:
                    {
                        SpaceStone spaceStone = stone as SpaceStone;
                        if(spaceStone == null)
                            continue;

                        if (_movementComponent.TotalMoveTime >= spaceStone.requiredMoveTime)
                        {
                            stoneState.UpdateCanUseStone(StoneType.Space, true);
                        }
                        break;
                    }
                    case StoneType.Time:
                    {
                        TimeStone timeStone = stone as TimeStone;
                        if(timeStone == null)
                            continue;
                        
                        _timeStoneCooldown -= Time.deltaTime;
                        
                        stoneState.UpdateCanUseStone(StoneType.Time, _timeStoneCooldown <= 0f);
                        break;
                    }
                    case StoneType.Reality:
                    {
                        RealityStone realityStone = stone as RealityStone;
                        if (realityStone != null)
                        {
                            stoneState.UpdateCanUseStone(StoneType.Reality,  _parryCount >= realityStone.RequiredParryCount);
                        }
                        break;
                    }
                }
            }
        }
        
        private void HandleParryEnd(bool isSuccess)
        {
            if(isSuccess)
                _parryCount++;
        }

        public void SwitchNext()
        {
            if (equipedStones == null)
                return;

            int equippedCount =
                equipedStones.Count(stone => stone != null);

            if (equippedCount <= 1)
                return;

            int previousIndex = _currentIndex;

            for (int i = 0; i < equipedStones.Length; i++)
            {
                _currentIndex++;

                if (_currentIndex >= equipedStones.Length)
                    _currentIndex = 0;

                if (equipedStones[_currentIndex] != null)
                {
                    Switch(previousIndex);
                    return;
                }
            }
        }

        public void SwitchPrevious()
        {
            if (equipedStones == null || equipedStones.Length == 0)
                return;

            int previousIndex = _currentIndex;

            for (int i = 0; i < equipedStones.Length; i++)
            {
                _currentIndex--;

                if (_currentIndex < 0)
                    _currentIndex = equipedStones.Length - 1;

                if (equipedStones[_currentIndex] != null)
                {
                    Switch(previousIndex);
                    return;
                }
            }
        }

        private void Switch(int previousIndex)
        {
            if (previousIndex < 0 ||
                previousIndex >= equipedStones.Length)
                return;

            if (_currentIndex < 0 ||
                _currentIndex >= equipedStones.Length)
                return;

            BaseStone previousStone =
                equipedStones[previousIndex];

            BaseStone currentStone =
                equipedStones[_currentIndex];

            // 이전 또는 현재 스톤이 없으면 스위칭하지 않음
            if (previousStone == null || currentStone == null)
                return;

            if (previousStone == currentStone)
                return;

            previousStone.OnSwitchExit(_player);
            currentStone.OnSwitchEnter(_player);

            Debug.Log(
                $"Stone Switch : " +
                $"{previousStone.StoneName} → " +
                $"{currentStone.StoneName}"
            );

            OnStoneChanged?.Invoke(
                previousStone,
                currentStone
            );

            TryStoneCombo(
                previousStone,
                currentStone
            );
        }

        private void TryStoneCombo(
            BaseStone previousStone,
            BaseStone currentStone)
        {
            Debug.Log(
                $"Stone Combo Check : " +
                $"{previousStone.StoneName} → " +
                $"{currentStone.StoneName}"
            );

            OnStoneCombo?.Invoke(
                previousStone,
                currentStone
            );
        }
        
        public void ResetTimeStoneCooldown(BaseStone timeStone)
        {
            if(CurrentStone == null || timeStone == null)
                return;

            if (timeStone is TimeStone time)
            {
                _timeStoneCooldown = time.Cooldown;
            }
        }
        
        public void Reset() { }
    }
}