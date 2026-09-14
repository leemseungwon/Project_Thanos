using System;

namespace LSW._02._Scripts.Common
{
    [Serializable]
    public enum MathOperation
    {
        Set, Add, Subtract, Multiply, Divide, Modulo
    }
    
    [Serializable]
    public enum EntityStatType
    {
        MoveSpeed,
        MaxHp,
        AttackDamage
    }

    [Serializable]
    public enum SceneType
    {
        Title
    }
    
    [Serializable]
    public enum RoomType
    {
        Start,
        Normal,
        Elite,
        Reward,
        Shop,
        Event,
        Boss
    }
}