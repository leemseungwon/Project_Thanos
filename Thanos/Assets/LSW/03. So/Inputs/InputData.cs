
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace LSW._03._So.Inputs
{
    [CreateAssetMenu(fileName = "Input Data", menuName = "So/InputData", order = 0)]
    public class InputData : ScriptableObject, PlayerInputAction.IPlayerActions
    {
        private PlayerInputAction _playerInputActions;
        private bool _isInputEnabled = true;

        public event Action OnAttackPressed;
        public event Action OnDashPressed;
        public event Action OnInteractionPressed;
        public event Action OnUseStonePressed;
        public event Action<bool> OnSwitchNextPressed; 
        
        public Vector2 MousePosition { get; private set; }
        public Vector2 MoveInput { get; private set; }
        
        private void OnEnable()
        {
            _isInputEnabled = true;
        }

        public void InitializeInput()
        {
            if (_playerInputActions == null)
                _playerInputActions = new PlayerInputAction();

            _playerInputActions.Player.SetCallbacks(this);
            _isInputEnabled = true;
            _playerInputActions.Enable();

            Debug.Log($"[InputData] Devices: {InputSystem.devices.Count}");
            Debug.Log($"[InputData] Move enabled: {_playerInputActions.Player.Move.enabled}");
        }
        
        public void EnableInput(bool enable)
        {
            _isInputEnabled = enable;
            if (!enable)
                MoveInput = Vector2.zero;
        }

        private void OnDisable()
        {
            _isInputEnabled = false;
            MoveInput = Vector2.zero;
            if (_playerInputActions != null)
            {
                _playerInputActions.Disable();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log($"[InputData] Move callback: {context.ReadValue<Vector2>()}");

            if (!_isInputEnabled)
                return;

            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Debug.Log($"[InputData] Look callback: {context.ReadValue<Vector2>()}");

            MousePosition = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(_isInputEnabled && context.started)
                OnAttackPressed?.Invoke();
        }

        public void OnUseStone(InputAction.CallbackContext context)
        {
            if(_isInputEnabled && context.started)
                OnUseStonePressed?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(_isInputEnabled && context.started)
                OnInteractionPressed?.Invoke();
        }

        public void OnSwitchNext(InputAction.CallbackContext context)
        {
            if(_isInputEnabled && context.started)
                OnSwitchNextPressed?.Invoke(true);
        }

        public void OnSwitchPrev(InputAction.CallbackContext context)
        {
            if(_isInputEnabled && context.started)
                OnSwitchNextPressed?.Invoke(false);
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if(_isInputEnabled && context.started)
                OnDashPressed?.Invoke();
        }
    }
}
