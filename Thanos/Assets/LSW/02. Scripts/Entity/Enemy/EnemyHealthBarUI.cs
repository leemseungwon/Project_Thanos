
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.Entity.Enemy
{
    [RequireComponent(typeof(BaseEnemy))]
    public class EnemyHealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Canvas canvas;
        
        private BaseEnemy _enemy;
        private EntityBuffComponent _buff;
        private bool _hiddenUntilRefresh;

        private void Awake()
        {
            _enemy = GetComponent<BaseEnemy>();
            _enemy.GetCompo(out _buff);
            
            if (canvas == null)
                return;

            if (canvas.worldCamera == null)
                canvas.worldCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (_enemy == null)
                _enemy = GetComponent<BaseEnemy>();

            if (_enemy != null)
                _enemy.OnHealthChanged += UpdateHealth;
        }

        private void OnDisable()
        {
            if (_enemy != null)
                _enemy.OnHealthChanged -= UpdateHealth;
        }

        public void Refresh()
        {
            _hiddenUntilRefresh = false;
            UpdateHealth(_enemy.CurrentHp, _enemy.MaxHp);
        }

        private void UpdateHealth(int currentHp, int maxHp)
        {
            if (healthSlider != null)
                healthSlider.value = maxHp > 0
                    ? Mathf.Clamp01(currentHp / (float)maxHp)
                    : 0f;
        }

        private static Image CreateImage(string objectName, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(objectName,
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);

            Image image = imageObject.GetComponent<Image>();
            image.sprite = WhiteSprite;
            image.color = color;
            return image;
        }

        private static Sprite _whiteSprite;
        private static Sprite WhiteSprite
        {
            get
            {
                if (_whiteSprite == null)
                {
                    Texture2D texture = Texture2D.whiteTexture;
                    _whiteSprite = Sprite.Create(texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                }

                return _whiteSprite;
            }
        }
    }
}
