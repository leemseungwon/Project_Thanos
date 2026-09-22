
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._03._So.Animation_Datas;
using LSW._03._So.Stone_Data;
using UnityEngine;

namespace LSW._02._Scripts.UI.InGame
{
    public class StoneUI : MonoBehaviour
    {
        [SerializeField] private Dictionary<StoneType, Animator> stoneImage;
        [SerializeField] private AnimationData canUseAnimationData;

        private HandlePlayerSystem _handlePlayerSystem;
        private PlayerStoneComponent _playerStoneComponent;
        
        private void Start()
        {
            if (SystemHandler.Instance.GetSystem(out _handlePlayerSystem))
            {
                if (_handlePlayerSystem.Player != null && _handlePlayerSystem.Player.GetCompo(out _playerStoneComponent))
                {
                    _playerStoneComponent.OnStoneChanged += UpdateChangedStone;
                    _playerStoneComponent.OnUpdateStoneUsable += UpdateStoneUI;
                }
            }
        }

        private void UpdateChangedStone(BaseStone prev, BaseStone cur)
        {
            if(cur == null)
                return;
            stoneImage[prev.StoneType].gameObject.SetActive(false);
            stoneImage[cur.StoneType].gameObject.SetActive(true);
        }

        public void UpdateStoneUI(BaseStone curStone, bool usable)
        {
            stoneImage[curStone.StoneType].SetBool(canUseAnimationData.animationHash, usable);
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