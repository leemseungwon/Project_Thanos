using System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.InGame
{
    public class InteractionPromptUI : MonoBehaviour, IPoolable
    {
        [SerializeField] private Button promptButton;
        [SerializeField] private Vector3 worldOffset = Vector3.up;

        private RectTransform _rectTransform;
        private Action _onPressed;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            if (promptButton == null)
                promptButton = GetComponentInChildren<Button>(true);
            if (promptButton != null)
                promptButton.onClick.AddListener(HandlePressed);
        }

        public void SetPrompt(Action onPressed, Vector3 worldPosition, Canvas canvas,
            Camera worldCamera)
        {
            _onPressed = onPressed;
            SetWorldPosition(worldPosition + worldOffset, canvas, worldCamera);
        }

        public void Initialize()
        {
            _rectTransform = transform as RectTransform;
            if (promptButton == null)
                promptButton = GetComponentInChildren<Button>(true);
            if (promptButton != null)
            {
                promptButton.onClick.RemoveListener(HandlePressed);
                promptButton.onClick.AddListener(HandlePressed);
            }
        }

        public void Spawn()
        {
            _onPressed = null;
        }

        public void Despawn()
        {
            _onPressed = null;
        }

        public void SetWorldPosition(Vector3 worldPosition, Canvas canvas, Camera worldCamera)
        {
            if (_rectTransform == null)
                _rectTransform = transform as RectTransform;
            if (_rectTransform == null)
                return;

            Canvas promptCanvas = GetComponent<Canvas>();
            if (promptCanvas != null && promptCanvas.renderMode == RenderMode.WorldSpace)
            {
                // Render prompts on the dedicated topmost UI sorting layer so
                // world sprites (for example the portal) cannot cover them.
                promptCanvas.overrideSorting = true;
                promptCanvas.sortingLayerName = "UI";
                promptCanvas.sortingOrder = 50;
                if (worldCamera != null)
                    promptCanvas.worldCamera = worldCamera;
                _rectTransform.position = worldPosition;
                return;
            }

            if (canvas == null)
                return;

            Camera cameraToUse = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera != null ? canvas.worldCamera : worldCamera;
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPosition);

            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        canvas.transform as RectTransform, screenPosition, cameraToUse,
                        out Vector3 worldCanvasPosition))
                {
                    _rectTransform.position = worldCanvasPosition;
                }
                return;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform, screenPosition, cameraToUse,
                    out Vector2 localPosition))
            {
                _rectTransform.anchoredPosition = localPosition;
            }
        }

        private void HandlePressed()
        {
            Action callback = _onPressed;
            _onPressed = null;
            callback?.Invoke();
        }

        private void OnDestroy()
        {
            if (promptButton != null)
                promptButton.onClick.RemoveListener(HandlePressed);
        }
    }
}
