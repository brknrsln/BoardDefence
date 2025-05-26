namespace Items
{
    public class EnemyType1 : Enemy
    {
        protected override void Awake()
        {
            TypeIndex = 0;
            base.Awake();
        }
    }
}