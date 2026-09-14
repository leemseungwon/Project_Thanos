using System;
using LSW._02._Scripts.Entity.Player;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerStoneComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Stones")]
        [SerializeField] private BaseStone[] stones;

        private PlayerController _player;
        private int _currentIndex;

        public int CurrentIndex => _currentIndex;

        public BaseStone CurrentStone
        {
            get
            {
                if (stones == null || stones.Length == 0)
                    return null;

                return stones[_currentIndex];
            }
        }

        public event Action<int, int> OnStoneChanged;
        public event Action<BaseStone, BaseStone> OnStoneCombo;

        public void Initialize(BaseEntity owner)
        {
            _player = owner as PlayerController;
            if (_player != null)
                _player.Input.OnUseStonePressed += UseStone;
            
            _currentIndex = 0;
        }

        private void UseStone()
        {
            if(CurrentStone == null)
                return;
            CurrentStone.Use(_player);
        }

        public void SwitchNext()
        {
            if (stones == null || stones.Length <= 1)
                return;

            int previousIndex = _currentIndex;

            _currentIndex++;

            if (_currentIndex >= stones.Length)
                _currentIndex = 0;

            Switch(previousIndex);
        }

        public void SwitchPrevious()
        {
            if (stones == null || stones.Length <= 1)
                return;

            int previousIndex = _currentIndex;

            _currentIndex--;

            if (_currentIndex < 0)
                _currentIndex = stones.Length - 1;

            Switch(previousIndex);
        }

        private void Switch(int previousIndex)
        {
            BaseStone previousStone = stones[previousIndex];
            BaseStone currentStone = stones[_currentIndex];

            if (previousStone == currentStone)
                return;

            previousStone.OnSwitchExit(_player);

            currentStone.OnSwitchEnter(_player);

            Debug.Log(
                $"Stone Switch : " +
                $"{previousStone.StoneName} ¡æ " +
                $"{currentStone.StoneName}"
            );

            OnStoneChanged?.Invoke(
                previousIndex,
                _currentIndex
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
                $"{previousStone.StoneName} ¡æ " +
                $"{currentStone.StoneName}"
            );

            OnStoneCombo?.Invoke(
                previousStone,
                currentStone
            );
        }
    }
}