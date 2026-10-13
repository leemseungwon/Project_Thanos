using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Scripts.UI.InGame
{
    public class PlayerStatUI : MonoBehaviour
    {
        [Header("Hp")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        
        [Header("Dash")]
        [SerializeField] private Slider dashSlider;
            
        public void UpdateUI(int currentHp, int maxHp, float dashCooldown)
        {
            hpSlider.value = currentHp / (float) maxHp;
            hpText.text = currentHp + " / " + maxHp;
            dashSlider.value = dashCooldown;
        }
    }
}