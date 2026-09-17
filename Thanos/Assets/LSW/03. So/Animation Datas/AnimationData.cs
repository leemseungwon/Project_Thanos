using UnityEngine;

namespace LSW._03._So.Animation_Datas
{
    [CreateAssetMenu(fileName = "Animation Hash", menuName = "So/Data/Animation/Animation Hash", order = 0)]
    public class AnimationData : ScriptableObject
    {
        public string animationName;
        public int animationHash;
        
        private void OnValidate()
        {
            animationHash = Animator.StringToHash(animationName);
        }
    }
}