namespace LSW._02._Scripts.Entity
{
    public interface IEntityComponent
    {
        public void Initialize(BaseEntity owner);
        public void Reset();
    }
}