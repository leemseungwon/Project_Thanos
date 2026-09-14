using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.Rooms
{
    public class BaseRoom : MonoBehaviour, IPoolable
    {
        public void Initialize()
        {
            
        }

        public void Spawn()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        public void Despawn()
        {
            
        }
    }
}