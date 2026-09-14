
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSW._03._So.Inputs
{
    [CreateAssetMenu(fileName = "Input Data", menuName = "So/InputData", order = 0)]
    public class InputData : ScriptableObject, PlayerInputAction.IPlayerActions
    {
        private PlayerInputAction _playerInputActions;

        public event Action OnAttackPressed;
        public event Action OnDashPressed;
        public event Action OnInteractionPressed;
        public event Action OnUseStonePressed;
        public event Action<bool> OnSwitchNextPressed; 
        
        public Vector2 MousePosition { get; private set; }
        public Vector2 MoveInput { get; private set; }
        
        private void OnEnable()
        {
            if (_playerInputActions == null)
            {
                _playerInputActions = new PlayerInputAction();
                _playerInputActions.Player.SetCallbacks(this);
            }
            EnableInput(true);
        }

        public void EnableInput(bool enable)
        {
            if (enable)
                _playerInputActions.Enable();
            else
                _playerInputActions.Disable();
        }
        

        private void OnDisable()
        {
            EnableInput(false);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.started)
                OnAttackPressed?.Invoke();
        }

        public void OnUseStone(InputAction.CallbackContext context)
        {
            if(context.started)
                OnUseStonePressed?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(context.started)
                OnInteractionPressed?.Invoke();
        }

        public void OnSwitchNext(InputAction.CallbackContext context)
        {
            if(context.started)
                OnSwitchNextPressed?.Invoke(true);
        }

        public void OnSwitchPrev(InputAction.CallbackContext context)
        {
            if(context.started)
                OnSwitchNextPressed?.Invoke(false);
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.started)
                OnDashPressed?.Invoke();
        }
    }
}