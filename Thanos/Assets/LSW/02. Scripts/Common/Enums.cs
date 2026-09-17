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
    
    [Serializable]
    public enum StoneType
    {
        Space,
        Time,
        Reality,
        Soul,
        Power,
        Mind
    }

    public enum SlimeAttackType
    {
        Front,
        Whirlwind
    }
}