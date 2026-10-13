
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSW._02._Scripts.UI.Rooms.Stone_Selections
{
    public class StoneSelectionUI : MonoBehaviour
    { 
        [SerializeField] private List<StoneSelectionChoiceUI> stoneButtons;
        
        private CanvasGroup canvasGroup;
        
        private Action onStoneSelectedCompleteAction;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            foreach (var btn in stoneButtons)
            {
                btn.OnStoneSelectUIEnd += Hide;
            }
        }

        public void Show(Action onStoneSelectedComplete = null)
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            onStoneSelectedCompleteAction = onStoneSelectedComplete;

            foreach (var btn in stoneButtons)
            {
                btn.SetInitialize();
            }
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            onStoneSelectedCompleteAction?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (var btn in stoneButtons)
            {
                btn.OnStoneSelectUIEnd -= Hide;
            }
        }
    }
}
