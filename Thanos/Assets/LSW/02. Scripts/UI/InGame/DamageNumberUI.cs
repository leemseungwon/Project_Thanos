using System.Collections;
using LSW._02._Scripts.System.PoolSystems;
using TMPro;
using UnityEngine;

namespace LSW._02._Scripts.UI.InGame
{
    public sealed class DamageNumberUI : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshPro damageText;
        [SerializeField, Min(0.01f)] private float fadeDuration = 0.8f;

        private Color _originalColor;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (damageText == null)
                damageText = GetComponentInChildren<TextMeshPro>(true);

            if (damageText == null)
                Debug.LogError("DamageNumberUI requires a 3D TextMeshPro component.", this);
            else
                _originalColor = damageText.color;
        }

        public void Initialize() { }

        public void Spawn()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (damageText != null)
            {
                damageText.color = _originalColor;
                damageText.text = string.Empty;
            }
        }

        public void SetDamage(int damage)
        {
            if (damageText == null)
            {
                Debug.LogError("Cannot display damage because no TextMeshPro text is assigned.", this);
                return;
            }

            if (damage <= 0)
            {
                Debug.LogWarning($"Damage number must be positive; received {damage}.", this);
                return;
            }

            if (fadeDuration <= 0f)
            {
                Debug.LogError("Damage number fade duration must be greater than zero.", this);
                return;
            }

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            damageText.text = damage.ToString();
            damageText.color = _originalColor;
            _fadeCoroutine = StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                Color color = _originalColor;
                color.a = _originalColor.a * (1f - Mathf.Clamp01(elapsed / fadeDuration));
                damageText.color = color;
                yield return null;
            }

            Color fadedColor = _originalColor;
            fadedColor.a = 0f;
            damageText.color = fadedColor;
            _fadeCoroutine = null;
            gameObject.SetActive(false);
        }

        public void Despawn()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
        }
    }
}
