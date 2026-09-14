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
}