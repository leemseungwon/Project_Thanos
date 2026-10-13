
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._03._So.Animation_Datas;
using LSW._03._So.Stone_Data;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.InGame
{
    public class StoneUI : MonoBehaviour
    {
        private static readonly int Fill = Shader.PropertyToID("_Fill");
        [SerializeField] private Dictionary<StoneType, Animator> stoneImage;
        [SerializeField] private AnimationData canUseAnimationData;

        private HandlePlayerSystem _handlePlayerSystem;
        private PlayerStoneComponent _playerStoneComponent;
        
        private void Start()
        {
            foreach (Animator animator in stoneImage.Values)
            {
                if (animator != null)
                    animator.gameObject.SetActive(false);
            }

            if (SystemHandler.Instance.GetSystem(out _handlePlayerSystem))
            {
                if (_handlePlayerSystem.Player != null && _handlePlayerSystem.Player.GetCompo(out _playerStoneComponent))
                {
                    _playerStoneComponent.OnStoneChanged += UpdateChangedStone;
                    _playerStoneComponent.OnUpdateStoneUsable += UpdateStoneUI;
                    _playerStoneComponent.RefreshCurrentStoneUI();
                }
            }
        }

        private void UpdateChangedStone(BaseStone prev, BaseStone cur)
        {
            if (prev != null)
                stoneImage[prev.StoneType].gameObject.SetActive(false);
            
            if(cur != null)
                stoneImage[cur.StoneType].gameObject.SetActive(true);
        }

        public void UpdateStoneUI(BaseStone curStone, float usablePercent, bool usable)
        {
            stoneImage[curStone.StoneType].SetBool(canUseAnimationData.animationHash, usable);
            
            if(stoneImage[curStone.StoneType].TryGetComponent(out Image image))
            {
                image.material.SetFloat(Fill, Mathf.Clamp01(usablePercent));
            }
        }

        private void OnDestroy()
        {
            if (_playerStoneComponent != null)
            {
                _playerStoneComponent.OnStoneChanged -= UpdateChangedStone;
                _playerStoneComponent.OnUpdateStoneUsable -= UpdateStoneUI;
            }
        }
    }
}