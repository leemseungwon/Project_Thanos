using System;
using System.Collections.Generic;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.StoneSystems;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._03._So.Stone_Data;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.Rooms.Reward_Selections
{
    public class RewardSelectionUI : MonoBehaviour
    {
        [Header("Upgrade Choices")]
        [SerializeField] private List<RewardSelectionChoiceUI> choices = new();

        [Header("Skip")]
        [SerializeField] private Button skipButton;

        private StoneSystem _stoneSystem;
        private CanvasGroup canvasGroup;
        private Action<BaseStoneUpgrade> _onSelected;
        private Action<int> _onChoiceSelected;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            SystemHandler systemHandler = SystemHandler.Instance;
            if (systemHandler != null)
                systemHandler.GetSystem(out _stoneSystem);

            foreach (RewardSelectionChoiceUI choice in choices)
            {
                if (choice != null)
                {
                    choice.OnUpgradeSelected += SelectUpgrade;
                    choice.OnRegularChoiceSelected += SelectChoice;
                    choice.OnRewardOptionSelected += SelectChoice;
                }
            }

            if (skipButton != null)
                skipButton.onClick.AddListener(SkipSelection);

            Hide();
        }

        public void ShowRandomUpgrades(Action<BaseStoneUpgrade> onSelected)
        {
            Hide();
            _onChoiceSelected = null;
            List<BaseStoneUpgrade> upgrades = new();

            if (_stoneSystem == null && SystemHandler.Instance != null)
                SystemHandler.Instance.GetSystem(out _stoneSystem);

            if (_stoneSystem != null)
            {
                PlayerStoneComponent playerStones = null;
                if (SystemHandler.Instance != null &&
                    SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem) &&
                    playerSystem.Player != null)
                    playerSystem.Player.GetCompo(out playerStones);

                int choiceCount = choices.Count > 0 ? choices.Count : 3;
                int maxAttempts = choiceCount * 10;
                int attempts = 0;

                while (upgrades.Count < choiceCount && attempts < maxAttempts)
                {
                    attempts++;

                    BaseStoneUpgrade upgrade = playerStones != null
                        ? _stoneSystem.GetRandomStoneUpgrade(playerStones)
                        : _stoneSystem.GetRandomStoneUpgrade();

                    if (upgrade != null && !upgrades.Contains(upgrade))
                        upgrades.Add(upgrade);
                }
            }

            if (choices.Count == 0)
            {
                if (upgrades.Count == 0)
                {
                    ShowChoices("No Stones to Upgrade", new[] { "OK" }, _ => onSelected?.Invoke(null));
                    return;
                }

                List<string> labels = new();
                foreach (BaseStoneUpgrade upgrade in upgrades)
                    labels.Add($"{upgrade.UpgradeName}\n{upgrade.Description}");
                ShowChoices("Choose a Stone Upgrade", labels, index =>
                    onSelected?.Invoke(index >= 0 && index < upgrades.Count ? upgrades[index] : null));
                return;
            }

            ShowUpgrades(upgrades, onSelected);
        }

        /// <summary>Displays immediate, non-animated choices for rewards, shops, and events.</summary>
        public void ShowChoices(string title, IReadOnlyList<string> optionLabels, Action<int> onSelected)
        {
            Hide();
            if (skipButton != null) skipButton.gameObject.SetActive(false);

            if (optionLabels == null)
            {
                onSelected?.Invoke(-1);
                return;
            }

            int optionCount = Mathf.Min(choices.Count, optionLabels.Count);
            bool hasChoice = false;
            for (int i = 0; i < optionCount; i++)
                hasChoice |= choices[i] != null;

            if (!hasChoice)
            {
                onSelected?.Invoke(-1);
                return;
            }

            _onChoiceSelected = onSelected;

            for (int i = 0; i < optionCount; i++)
            {
                RewardSelectionChoiceUI choice = choices[i];
                if (choice == null)
                    continue;

                choice.SetRegularChoice(optionLabels[i], i);
                choice.gameObject.SetActive(true);
            }

            SetVisible(true);
        }

        public void ShowRewardOptions(string title, IReadOnlyList<BaseUpgradeSo> options, Action<int> onSelected)
        {
            if (options == null || options.Count == 0)
            {
                onSelected?.Invoke(-1);
                return;
            }

            if (choices.Count == 0)
            {
                List<string> labels = new();
                foreach (BaseUpgradeSo option in options)
                    labels.Add(option != null ? $"{option.Title}\n{option.Description}" : string.Empty);
                ShowChoices(title, labels, onSelected);
                return;
            }

            Hide();
            if (skipButton != null)
                skipButton.gameObject.SetActive(false);

            _onChoiceSelected = onSelected;

            int visibleCount = Mathf.Min(options.Count, choices.Count);
            int activeCount = 0;
            for (int i = 0; i < choices.Count; i++)
            {
                RewardSelectionChoiceUI choice = choices[i];
                bool visible = choice != null && i < visibleCount && options[i] != null;
                if (!visible)
                {
                    if (choice != null)
                        choice.gameObject.SetActive(false);
                    continue;
                }

                choice.SetRewardOption(options[i], i);
                choice.gameObject.SetActive(true);
                activeCount++;
            }

            SetVisible(activeCount > 0);
        }

        private void SelectChoice(int index)
        {
            Action<int> callback = _onChoiceSelected;
            Hide();
            callback?.Invoke(index);
        }

        private void HideChoices()
        {
            foreach (RewardSelectionChoiceUI choice in choices)
                if (choice != null)
                    choice.gameObject.SetActive(false);
        }

        private void ShowUpgrades(IReadOnlyList<BaseStoneUpgrade> upgrades,
            Action<BaseStoneUpgrade> onSelected)
        {
            _onSelected = onSelected;

            int visibleCount = upgrades == null
                ? 0
                : Mathf.Min(upgrades.Count, choices.Count);

            int selectableCount = 0;

            for (int i = 0; i < choices.Count; i++)
            {
                RewardSelectionChoiceUI choice = choices[i];
                if (choice == null)
                    continue;

                bool hasUpgrade = upgrades != null &&
                                  i < visibleCount &&
                                  upgrades[i] != null;

                choice.gameObject.SetActive(hasUpgrade);

                if (hasUpgrade && choice.SetUpgrade(upgrades[i]))
                    selectableCount++;
            }

            bool showSkip = selectableCount == 0;

            if (skipButton != null)
                skipButton.gameObject.SetActive(showSkip);

            bool showUI = visibleCount > 0 || showSkip;

            SetVisible(showUI);
        }

        public void Hide()
        {
            _onSelected = null;
            _onChoiceSelected = null;
            HideChoices();

            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private void SelectUpgrade(BaseStoneUpgrade upgrade)
        {
            Action<BaseStoneUpgrade> callback = _onSelected;

            Hide();
            callback?.Invoke(upgrade);
        }

        private void SkipSelection()
        {
            Action<BaseStoneUpgrade> callback = _onSelected;

            Hide();
            callback?.Invoke(null);
        }

        private void OnDestroy()
        {
            foreach (RewardSelectionChoiceUI choice in choices)
            {
                if (choice != null)
                {
                    choice.OnUpgradeSelected -= SelectUpgrade;
                    choice.OnRegularChoiceSelected -= SelectChoice;
                    choice.OnRewardOptionSelected -= SelectChoice;
                }
            }

            if (skipButton != null)
                skipButton.onClick.RemoveListener(SkipSelection);
        }
    }
}
