using UnityEngine;
using UnityEngine.Serialization;
using LSW._02._Scripts.Entity.Player;

namespace LSW._03._So.Stone_Data
{
    public abstract class BaseUpgradeSo : ScriptableObject
    {
        [Header("Reward Info")]
        [FormerlySerializedAs("stoneSprite")]
        [SerializeField] private Sprite icon;

        [FormerlySerializedAs("stoneName"), FormerlySerializedAs("upgradeName")]
        [SerializeField] private string title;

        [FormerlySerializedAs("stoneDescription"), FormerlySerializedAs("description")]
        [TextArea, SerializeField] private string description;

        public Sprite Icon => icon;
        public string Title => title;
        public string Description => description;

        public abstract bool Apply(PlayerController player);
    }
}
