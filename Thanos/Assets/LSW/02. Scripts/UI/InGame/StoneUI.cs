
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.HandlePlayerSystems;
using LSW._03._So.Stone_Data;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.InGame
{
    public class StoneUI : MonoBehaviour
    {
        [SerializeField] private Image stoneImage;

        private HandlePlayerSystem _handlePlayerSystem;
        private PlayerStoneComponent _playerStoneComponent;
        
        private void Start()
        {
            if (SystemHandler.Instance.GetSystem(out _handlePlayerSystem))
            {
                if (_handlePlayerSystem.Player != null && _handlePlayerSystem.Player.GetCompo(out _playerStoneComponent))
                {
                    _playerStoneComponent.OnStoneChanged += UpdateChangedStone;
                }
            }
        }

        private void UpdateChangedStone(BaseStone prev, BaseStone cur)
        {
            if(cur == null)
                return;
            stoneImage.sprite = cur.StoneSprite;
        }

        private void OnDestroy()
        {
            if (_playerStoneComponent != null)
            {
                _playerStoneComponent.OnStoneChanged -= UpdateChangedStone;
            }
        }
    }
}