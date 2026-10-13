using System.Collections;
using LSW._02._Scripts.System;
using LSW._02._Scripts.System.PoolSystems;
using UnityEngine;

namespace LSW._02._Scripts.Environment.Particle
{
    public class PoolingParticle : MonoBehaviour, IPoolable
    {
        [SerializeField] private string poolName;
        [SerializeField] private float duration;
        
        private Coroutine _coroutine;
        private ParticleSystem[] particleSystems;

        public void Initialize()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        public void Spawn()
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);

            foreach (var particleSystem in particleSystems)
            {
                particleSystem.Play();
            }
            _coroutine = StartCoroutine(DespawnAfterDuration());
        }

        private IEnumerator DespawnAfterDuration()
        {
            yield return new WaitForSeconds(duration);
            if (SystemHandler.Instance.GetSystem(out PoolSystem poolSystem))
            {
                poolSystem.DespawnPool(poolName, this);
            }
        }

        public void Despawn()
        {
            foreach (var particleSystem in particleSystems)
            {
                particleSystem.Stop();
            }
        }
    }
}