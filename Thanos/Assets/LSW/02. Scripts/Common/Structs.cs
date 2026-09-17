using System;
using LSW._02._Scripts.Entity;
using UnityEngine;

namespace LSW._02._Scripts.Common
{
    [Serializable]
    public struct StateData
    {
        public string stateName;
        public EntityState State;
    }
    
    [Serializable]
    public struct PoolingData
    {
        public string poolName;
        public GameObject poolObject;
        public int initialPoolSize;
    }

    public struct ParryData
    {
        public readonly int ReflectDamage;
        public readonly float KnockbackPower;
        public readonly float StunDuration;
        
        public ParryData(int reflectDamage, float knockbackPower, float stunDuration)
        {
            ReflectDamage = reflectDamage;
            KnockbackPower = knockbackPower;
            StunDuration = stunDuration;
        }
    }
}