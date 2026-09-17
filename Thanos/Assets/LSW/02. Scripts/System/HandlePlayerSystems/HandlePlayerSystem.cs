using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player;
using LSW._02._Scripts.UI.InGame;
using UnityEngine;

namespace LSW._02._Scripts.System.HandlePlayerSystems
{
    [SystemOrder(-1)]
    public class HandlePlayerSystem : MonoBehaviour, ISystem
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private InGameUI inGameUI;
        
        public PlayerController Player => playerController;
        public Camera Camera => playerController.PlayerCamera;
        public InGameUI InGameUI => inGameUI;
        
        public void Initialize(SystemHandler systemHandler) { }
        
        public void Reset() { }
    }
}