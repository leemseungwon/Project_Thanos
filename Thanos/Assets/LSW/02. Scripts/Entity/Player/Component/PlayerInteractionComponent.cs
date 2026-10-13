
using System.Collections.Generic;
using LSW._02._Scripts.Environment.Interactable;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using LSW._02._Scripts.UI.InGame;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player.Component
{
    public class PlayerInteractionComponent : MonoBehaviour, IEntityComponent
    {
        [Header("Interaction")]
        [SerializeField] private float interactionRadius = 2f;
        [SerializeField] private float portalPriorityRadius = 1.25f;
        [SerializeField] private LayerMask interactionLayer;
        
        private PlayerController _playerController;
        private Canvas _interactionCanvas;
        private string _interactionPromptPoolName;
        private Camera _worldCamera;
        private PoolSystem _poolSystem;
        private readonly Dictionary<UnityEngine.Component, InteractionPromptUI> _prompts = new();
        private readonly HashSet<UnityEngine.Component> _targetsInRange = new();
        private readonly List<UnityEngine.Component> _targetsToHide = new();
        
        public void Initialize(BaseEntity owner)
        {
            _playerController = owner as PlayerController;
            if (_playerController != null)
            {
                _playerController.Input.OnInteractionPressed += TryInteract;
            }
        }

        private void Start()
        {
            InGameUI inGameUI = FindFirstObjectByType<InGameUI>();
            if (inGameUI != null)
            {
                _interactionCanvas = inGameUI.InteractionCanvas != null
                    ? inGameUI.InteractionCanvas
                    : inGameUI.GetComponentInParent<Canvas>();
                _interactionPromptPoolName = inGameUI.InteractionPromptPoolName;
            }

            if (SystemHandler.Instance != null)
                SystemHandler.Instance.GetSystem(out _poolSystem);

            _worldCamera = _playerController.PlayerCamera != null
                ? _playerController.PlayerCamera
                : Camera.main;
        }

        private void LateUpdate()
        {
            RefreshInteractionPrompts();
        }

        private void RefreshInteractionPrompts()
        {
            if (string.IsNullOrEmpty(_interactionPromptPoolName) || _poolSystem == null)
            {
                HideAllPrompts();
                return;
            }

            _targetsInRange.Clear();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                transform.position, interactionRadius, interactionLayer);

            foreach (Collider2D col in colliders)
            {
                IInteractable interactable = col.GetComponentInParent<IInteractable>();
                if (interactable == null ||
                    interactable is RewardPickup pickup && !pickup.CanCollect)
                    continue;

                UnityEngine.Component target = interactable as UnityEngine.Component;
                if (target == null)
                    continue;

                Vector3 targetPosition = target.transform.position;
                Vector3 screenPosition = _worldCamera != null
                    ? _worldCamera.WorldToScreenPoint(targetPosition)
                    : targetPosition;
                if (_worldCamera != null && screenPosition.z <= 0f)
                    continue;

                if (!_targetsInRange.Add(target))
                    continue;

                if (!_prompts.TryGetValue(target, out InteractionPromptUI prompt) || prompt == null)
                {
                    if (!_poolSystem.SpawnPool(_interactionPromptPoolName, out prompt))
                        continue;

                    prompt.transform.SetParent(null, false);
                    _prompts[target] = prompt;
                }

                prompt.SetPrompt(() => InteractWith(interactable), targetPosition,
                    _interactionCanvas, _worldCamera);
            }

            _targetsToHide.Clear();
            foreach (KeyValuePair<UnityEngine.Component, InteractionPromptUI> pair in _prompts)
            {
                if (pair.Key == null || !_targetsInRange.Contains(pair.Key))
                    _targetsToHide.Add(pair.Key);
            }

            foreach (UnityEngine.Component target in _targetsToHide)
                DespawnPrompt(target);
        }

        private void HideAllPrompts()
        {
            foreach (InteractionPromptUI prompt in _prompts.Values)
            {
                if (prompt == null || !prompt.gameObject.activeSelf)
                    continue;

                if (_poolSystem != null)
                    _poolSystem.DespawnPool(_interactionPromptPoolName, prompt);
                else
                    prompt.Despawn();
            }
            _prompts.Clear();
        }

        private void InteractWith(IInteractable interactable)
        {
            if (interactable is UnityEngine.Component target)
                DespawnPrompt(target);

            interactable.Interact();
        }

        private void DespawnPrompt(UnityEngine.Component target)
        {
            if (!_prompts.TryGetValue(target, out InteractionPromptUI prompt))
                return;

            _prompts.Remove(target);
            if (prompt == null || !prompt.gameObject.activeSelf)
                return;

            if (_poolSystem != null)
                _poolSystem.DespawnPool(_interactionPromptPoolName, prompt);
            else
                prompt.Despawn();
        }

        private void TryInteract()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                    transform.position, interactionRadius, interactionLayer);

            if (colliders.Length == 0)
                return;

            float closestDistance = float.MaxValue;
            IInteractable closestInteractable = null;
            float closestPortalDistance = float.MaxValue;
            IInteractable closestPortal = null;

            foreach (Collider2D col in colliders)
            {
                IInteractable interactable =
                    col.GetComponentInParent<IInteractable>();

                if (interactable == null)
                    continue;

                if (interactable is RewardPickup pickup && !pickup.CanCollect)
                    continue;

                float distance = (col.transform.position -
                                  transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }

                if (interactable is Portal && distance < closestPortalDistance)
                {
                    closestPortalDistance = distance;
                    closestPortal = interactable;
                }
            }

            if (closestPortal != null &&
                closestPortalDistance <= portalPriorityRadius * portalPriorityRadius)
            {
                InteractWith(closestPortal);
                return;
            }

            if (closestInteractable == null)
                return;

            InteractWith(closestInteractable);
        }

        public void Reset() { }
        
        private void OnDestroy()
        {
            HideAllPrompts();
            if (_playerController != null)
            {
                _playerController.Input.OnInteractionPressed -= TryInteract;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position,
                interactionRadius);
        }
    }
}
