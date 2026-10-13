using System;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.StoneSystems;
using LSW._03._So.Stone_Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.Rooms.Reward_Selections
{
    public class RewardSelectionChoiceUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image stoneIcon;
        [SerializeField] private TextMeshProUGUI upgradeName;
        [SerializeField] private TextMeshProUGUI upgradeDescription;
        [SerializeField] private Image alreadyOwnImage;

        private Button _button;
        private StoneSystem _stoneSystem;
        private PlayerStoneComponent _playerStoneComponent;
        private BaseStoneUpgrade _upgrade;
        private bool _isRegularChoice;
        private bool _isRewardOption;
        private int _choiceIndex;
        
        public event Action<BaseStoneUpgrade> OnUpgradeSelected;
        public event Action<int> OnRegularChoiceSelected;
        public event Action<int> OnRewardOptionSelected;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SelectUpgrade);
        }

        private void Start()
        {
            SystemHandler systemHandler = SystemHandler.Instance;
            if (systemHandler != null)
            {
                systemHandler.GetSystem(out _stoneSystem);

                if (systemHandler.GetSystem(out HandlePlayerSystem playerSystem) &&
                    playerSystem.Player != null)
                {
                    playerSystem.Player.GetCompo(out _playerStoneComponent);
                }
            }
        }

        public bool SetUpgrade(BaseStoneUpgrade upgrade)
        {
            _isRegularChoice = false;
            _isRewardOption = false;
            _upgrade = upgrade;

            RefreshPlayerStoneComponent();
            
            if (_upgrade == null)
            {
                _button.interactable = false;
                return false;
            }

            BaseStone stone = _stoneSystem != null
                ? _stoneSystem.GetStone(_upgrade.StoneType)
                : null;

            SetDisplay(_upgrade, stone != null ? stone.StoneSprite : null);

            bool canSelect = _playerStoneComponent != null &&
                             _playerStoneComponent.HasStone(_upgrade.StoneType);

            _button.interactable = canSelect;

            bool hasUpgrade = _playerStoneComponent != null && 
                              _playerStoneComponent.HasUpgrade(_upgrade.StoneType, _upgrade, _upgrade.GetType());
            alreadyOwnImage.gameObject.SetActive(hasUpgrade);
            
            return canSelect;
        }

        private void RefreshPlayerStoneComponent()
        {
            _playerStoneComponent = null;

            SystemHandler systemHandler = SystemHandler.Instance;
            if (systemHandler != null &&
                systemHandler.GetSystem(out HandlePlayerSystem playerSystem) &&
                playerSystem.Player != null)
            {
                playerSystem.Player.GetCompo(out _playerStoneComponent);
            }
        }

        public void SetRewardOption(BaseUpgradeSo reward, int index)
        {
            _isRegularChoice = false;
            _isRewardOption = true;
            _choiceIndex = index;
            _upgrade = reward as BaseStoneUpgrade;

            Sprite fallbackIcon = _upgrade != null && _stoneSystem != null
                ? _stoneSystem.GetStone(_upgrade.StoneType)?.StoneSprite
                : null;
            SetDisplay(reward, fallbackIcon);

            if (_button != null)
                _button.interactable = reward != null;
            
            alreadyOwnImage.gameObject.SetActive(false);
        }

        public void SetRegularChoice(string label, int index)
        {
            _isRegularChoice = true;
            _isRewardOption = false;
            _upgrade = null;
            _choiceIndex = index;

            int separatorIndex = label != null ? label.IndexOf('\n') : -1;
            string title = separatorIndex >= 0 ? label.Substring(0, separatorIndex) : label;
            string description = separatorIndex >= 0
                ? label.Substring(separatorIndex + 1)
                : string.Empty;

            if (stoneIcon != null)
                stoneIcon.gameObject.SetActive(false);
            if (upgradeName != null)
                upgradeName.SetText(title);
            if (upgradeDescription != null)
                upgradeDescription.SetText(description);
            if (_button != null)
                _button.interactable = true;
            
            alreadyOwnImage.gameObject.SetActive(false);
        }

        private void SetDisplay(BaseUpgradeSo data, Sprite fallbackIcon)
        {
            if (stoneIcon != null)
            {
                stoneIcon.gameObject.SetActive(true);
                stoneIcon.sprite = data != null && data.Icon != null ? data.Icon : fallbackIcon;
            }
            if (upgradeName != null)
                upgradeName.SetText(data != null ? data.Title : string.Empty);
            if (upgradeDescription != null)
                upgradeDescription.SetText(data != null ? data.Description : string.Empty);
        }

        private void SelectUpgrade()
        {
            if (_isRegularChoice)
            {
                OnRegularChoiceSelected?.Invoke(_choiceIndex);
                return;
            }

            if (_isRewardOption)
            {
                OnRewardOptionSelected?.Invoke(_choiceIndex);
                return;
            }

            if (_upgrade != null && _button.interactable)
                OnUpgradeSelected?.Invoke(_upgrade);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(SelectUpgrade);
        }
    }
}
