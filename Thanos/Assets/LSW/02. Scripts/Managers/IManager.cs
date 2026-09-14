using LSW._02._Scripts.Common;

namespace LSW._02._Scripts.Managers
{
    public interface IManager
    {
        public void Initialize(ManagerHandler managerHandler);
        public void LoadScene(SceneType sceneType);
        public void Reset();
    }
}