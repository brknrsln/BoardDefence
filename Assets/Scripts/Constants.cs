using System.Collections.Generic;

public static class Constants
{
    public const int BoardRows = 8;
    public const int BoardColumns = 4;
    public const float CellSpacing = 1.1f;
    public static int EnemySize => EnemyDataDictionary.Count;

    public enum ObjectItemType
    {
        Enemy1 = 0,
        Enemy2 = 1,
        Enemy3 = 2,
        
        DefenceItem1 = 1000,
        DefenceItem2 = 1001,
        DefenceItem3 = 1002,
        
        BulletItem1 = 2000
    }
    
    public static List<ObjectItemType> EnemyTypes => new()
    {
        ObjectItemType.Enemy1,
        ObjectItemType.Enemy2,
        ObjectItemType.Enemy3
    };
    
    public static List<ObjectItemType> DefenceItemTypes => new()
    {
        ObjectItemType.DefenceItem1,
        ObjectItemType.DefenceItem2,
        ObjectItemType.DefenceItem3
    };
    
    public struct EnemyData
    {
        public readonly ObjectItemType ObjectItemType;
        public readonly int Health;
        public readonly float Speed;
        
        public EnemyData(ObjectItemType objectItemType, int health, float speed)
        {
            ObjectItemType = objectItemType;
            Health = health;
            Speed = speed;
        }
    }
    
    public static readonly Dictionary<int, EnemyData> EnemyDataDictionary = new()
    {
        { 0, new EnemyData(ObjectItemType.Enemy1, 3, 1.0f) },
        { 1, new EnemyData(ObjectItemType.Enemy2, 10, 0.25f) },
        { 2, new EnemyData(ObjectItemType.Enemy3, 5, 0.5f) }
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
        public readonly ObjectItemType ObjectItemType;
        public readonly int Damage;
        public readonly int Range;
        public readonly float AttackIntervalInSeconds;
        public readonly AttackDirection AttackDirection;
        
        public DefenceItemData(
            ObjectItemType objectItemType, int damage, int range, float attackIntervalInSeconds, AttackDirection attackDirection)
        {
            ObjectItemType = objectItemType;
            Damage = damage;
            Range = range;
            AttackIntervalInSeconds = attackIntervalInSeconds;
            AttackDirection = attackDirection;
        }
    }
    
    public static readonly Dictionary<int, DefenceItemData> DefenceItemDataDictionary = new()
    {
        { 0, 
            new DefenceItemData(ObjectItemType.DefenceItem1, 3, 4, 3.0f, AttackDirection.Forward) },
        { 1,
            new DefenceItemData(ObjectItemType.DefenceItem2, 5, 2, 4.0f, AttackDirection.Forward) },
        { 2, 
            new DefenceItemData(ObjectItemType.DefenceItem3, 10, 1, 5.0f, AttackDirection.All) }
    };
    
    public struct LevelData
    {
        public readonly int Level;
        public readonly Dictionary<ObjectItemType, int> TypeSizeDictionary;
        
        public LevelData(int level, Dictionary<ObjectItemType, int> typeSizeDictionary)
        {
            Level = level;
            TypeSizeDictionary = typeSizeDictionary;
        }
    }
    
    public static readonly Dictionary<int, LevelData> LevelDataDictionary = new()
    {
        { 1, new LevelData(1, new Dictionary<ObjectItemType, int>
            {
                { ObjectItemType.DefenceItem1, 3 },
                { ObjectItemType.DefenceItem2, 2 },
                { ObjectItemType.DefenceItem3, 1 },
                { ObjectItemType.Enemy1, 3 },
                { ObjectItemType.Enemy2, 1 },
                { ObjectItemType.Enemy3, 1 }
            }) }
        , { 2, new LevelData(2, new Dictionary<ObjectItemType, int>
            {
                { ObjectItemType.DefenceItem1, 3 },
                { ObjectItemType.DefenceItem2, 4 },
                { ObjectItemType.DefenceItem3, 2 },
                { ObjectItemType.Enemy1, 5 },
                { ObjectItemType.Enemy2, 2 },
                { ObjectItemType.Enemy3, 3 }
            }) }
        , { 3, new LevelData(3, new Dictionary<ObjectItemType, int>
            {
                { ObjectItemType.DefenceItem1, 5 },
                { ObjectItemType.DefenceItem2, 7 },
                { ObjectItemType.DefenceItem3, 5 },
                { ObjectItemType.Enemy1, 7 },
                { ObjectItemType.Enemy2, 3 },
                { ObjectItemType.Enemy3, 5 }
            }) }
    };
    
}