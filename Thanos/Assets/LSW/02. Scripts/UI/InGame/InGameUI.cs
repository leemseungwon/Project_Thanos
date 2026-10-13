
using LSW._02._Scripts.UI.Rooms;
using LSW._02._Scripts.UI.Rooms.Reward_Selections;
using LSW._02._Scripts.UI.Rooms.Stone_Selections;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.RoomSystems;
using LSW._02._Scripts.Entity.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSW._02._Scripts.UI.InGame
{
    public class InGameUI : MonoBehaviour
    {
        [field:SerializeField] public RoomSelectUI RoomSelectUI { get; private set;}
        [field:SerializeField] public StoneUI StoneUI { get; private set; }
        [field:SerializeField] public StoneSelectionUI StoneSelectionUI { get; private set; }
        [field:SerializeField] public RewardSelectionUI RewardSelectionUI { get; private set; }
        [field:SerializeField] public PlayerStatUI PlayerStatUI { get; private set; }
        [field:SerializeField] public string InteractionPromptPoolName { get; private set; } = "UI_InteractionPrompt";
        [field:SerializeField] public Canvas InteractionCanvas { get; private set; }

        private PlayerController _player;
        private RoomSystem _roomSystem;

        private void Start()
        {
            if (InteractionCanvas == null)
                InteractionCanvas = GetComponentInParent<Canvas>();
            BindPlayer();
            BindRoomSystem();
        }

        private void Update()
        {
            if (_player == null)
                BindPlayer();
            if (_roomSystem == null)
                BindRoomSystem();
        }

        private void BindPlayer()
        {
            if (SystemHandler.Instance == null ||
                !SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem) ||
                playerSystem.Player == null)
                return;

            if (_player != null)
            {
                _player.OnCurrencyChanged -= UpdateCurrency;
                _player.OnDeathEvent -= HandlePlayerDied;
                _player.OnStatChanged -= UpdatePlayerStat;
            }

            _player = playerSystem.Player;
            _player.OnCurrencyChanged += UpdateCurrency;
            _player.OnDeathEvent += HandlePlayerDied;
            _player.OnStatChanged += UpdatePlayerStat;
            UpdateCurrency(_player.Currency);
        }

        private void BindRoomSystem()
        {
            if (SystemHandler.Instance == null ||
                !SystemHandler.Instance.GetSystem(out _roomSystem) || _roomSystem == null)
                return;

            _roomSystem.OnRunWon -= HandleRunWon;
            _roomSystem.OnRunWon += HandleRunWon;
        }

        private void HandlePlayerDied()
        {
            _player?.EnableInput(false);
            RewardSelectionUI?.ShowChoices("Game Over", new[] { "Restart" }, RestartRun);
        }

        private void HandleRunWon()
        {
            _player?.EnableInput(false);
            RewardSelectionUI?.ShowChoices("Final Boss Defeated!", new[] { "Restart" }, RestartRun);
        }

        private void RestartRun(int _)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex >= 0)
                SceneManager.LoadScene(activeScene.buildIndex);
            else
                SceneManager.LoadScene(activeScene.name);
        }

        private void UpdateCurrency(int amount)
        {
            
        }
        
        public void UpdatePlayerStat(int currentHp, int maxHp, float dashCooldown)
        {
            PlayerStatUI.UpdateUI(currentHp, maxHp, dashCooldown);
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.OnCurrencyChanged -= UpdateCurrency;
                _player.OnDeathEvent -= HandlePlayerDied;
                _player.OnStatChanged -= UpdatePlayerStat;
            }
            if (_roomSystem != null)
                _roomSystem.OnRunWon -= HandleRunWon;
        }
    }
}
