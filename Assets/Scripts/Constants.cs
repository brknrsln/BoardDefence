using System.Collections.Generic;

public static class Constants
{
    public const int BoardRows = 8;
    public const int BoardColumns = 4;
    public static int EnemySize => EnemyDataDictionary.Count;

    public enum Type
    {
        Enemy1 = 0,
        Enemy2 = 1,
        Enemy3 = 2,
        
        DefenceItem1 = 1000,
        DefenceItem2 = 1001,
        DefenceItem3 = 1002,
        
        BulletItem1 = 2000
    }
    
    public static List<Type> EnemyTypes => new List<Type>
    {
        Type.Enemy1,
        Type.Enemy2,
        Type.Enemy3
    };
    
    public static List<Type> DefenceItemTypes => new List<Type>
    {
        Type.DefenceItem1,
        Type.DefenceItem2,
        Type.DefenceItem3
    };
    
    public struct EnemyData
    {
        public readonly Type Type;
        public readonly int Health;
        public readonly float Speed;
        
        public EnemyData(Type type, int health, float speed)
        {
            Type = type;
            Health = health;
            Speed = speed;
        }
    }
    
    public static readonly Dictionary<int, EnemyData> EnemyDataDictionary = new()
    {
        { 0, new EnemyData(Type.Enemy1, 3, 1.0f) },
        { 1, new EnemyData(Type.Enemy2, 10, 0.25f) },
        { 2, new EnemyData(Type.Enemy3, 5, 0.5f) }
    };
    
    public enum AttackDirection
    {
        Forward,
        Left,
        Backward,
        Right,
        All,
    }

    public struct DefenceItemData
    {
        public readonly Type Type;
        public readonly int Damage;
        public readonly int Range;
        public readonly float AttackIntervalInSeconds;
        public readonly AttackDirection AttackDirection;
        
        public DefenceItemData(
            Type type, int damage, int range, float attackIntervalInSeconds, AttackDirection attackDirection)
        {
            Type = type;
            Damage = damage;
            Range = range;
            AttackIntervalInSeconds = attackIntervalInSeconds;
            AttackDirection = attackDirection;
        }
    }
    
    public static readonly Dictionary<int, DefenceItemData> DefenceItemDataDictionary = new()
    {
        { 0, 
            new DefenceItemData(Type.DefenceItem1, 3, 4, 3.0f, AttackDirection.Forward) },
        { 1,
            new DefenceItemData(Type.DefenceItem2, 5, 2, 4.0f, AttackDirection.Forward) },
        { 2, 
            new DefenceItemData(Type.DefenceItem3, 10, 1, 5.0f, AttackDirection.All) }
    };
    
    public struct LevelData
    {
        public readonly int Level;
        public readonly Dictionary<Type, int> TypeSizeDictionary;
        
        public LevelData(int level, Dictionary<Type, int> typeSizeDictionary)
        {
            Level = level;
            TypeSizeDictionary = typeSizeDictionary;
        }
    }
    
    public static readonly Dictionary<int, LevelData> LevelDataDictionary = new()
    {
        { 1, new LevelData(1, new Dictionary<Type, int>
            {
                { Type.DefenceItem1, 3 },
                { Type.DefenceItem2, 2 },
                { Type.DefenceItem3, 1 },
                { Type.Enemy1, 3 },
                { Type.Enemy2, 1 },
                { Type.Enemy3, 1 }
            }) }
        , { 2, new LevelData(2, new Dictionary<Type, int>
            {
                { Type.DefenceItem1, 3 },
                { Type.DefenceItem2, 4 },
                { Type.DefenceItem3, 2 },
                { Type.Enemy1, 5 },
                { Type.Enemy2, 2 },
                { Type.Enemy3, 3 }
            }) }
        , { 3, new LevelData(3, new Dictionary<Type, int>
            {
                { Type.DefenceItem1, 5 },
                { Type.DefenceItem2, 7 },
                { Type.DefenceItem3, 5 },
                { Type.Enemy1, 7 },
                { Type.Enemy2, 3 },
                { Type.Enemy3, 5 }
            }) }
    };
    
    public enum GameState
    {
        Start,
        Playing,
        Paused,
        GameOver
    }
}