
using System;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._02._Scripts.System.StoneSystems;
using LSW._03._So.Stone_Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.Rooms.Stone_Selections
{
	[RequireComponent(typeof(Button))]
    public class StoneSelectionChoiceUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image stoneIcon;
        [SerializeField] private TextMeshProUGUI stoneName;
        [SerializeField] private TextMeshProUGUI stoneDescription;
            
        private Button _button;
        private StoneSystem stoneSystem;
        private PlayerStoneComponent stoneComponent;
        
        private BaseStone selectedStone;
        
        public event Action OnStoneSelectUIEnd;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            
            if (SystemHandler.Instance.GetSystem(out HandlePlayerSystem playerSystem))
            {
                playerSystem.Player.GetCompo(out stoneComponent);
            }

            SystemHandler.Instance.GetSystem(out stoneSystem);
        }

        private void Start()
        {
            if(_button != null)
                _button.onClick.AddListener(SelectStone);
        }

        public void SetInitialize()
        {
            if (stoneSystem != null)
            {
                selectedStone = stoneSystem.GetRandomStone();
            }
            SetUI();
        }

        private void SetUI()
        {
            if(stoneIcon != null)
                stoneIcon.sprite = selectedStone.StoneSprite;
            if(stoneName != null)
                stoneName.SetText(selectedStone.StoneName);
            if(stoneDescription != null)
                stoneDescription.SetText(selectedStone.StoneDescription);
        }
        
        private void SelectStone()
        {
            if (stoneComponent != null)
            {
                stoneComponent.GetStone(selectedStone);
            }
            
            OnStoneSelectUIEnd?.Invoke();
        }

        private void OnDestroy()
        {
            if(_button != null)
                _button.onClick.RemoveListener(SelectStone);
        }
    }
}
