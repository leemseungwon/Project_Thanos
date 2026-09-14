namespace LSW._02._Scripts.Entity
{
    public abstract class EntityState
    {
        protected readonly BaseEntity _entity;

        protected EntityState(BaseEntity entity)
        {
            _entity = entity;
        }

        public virtual void Enter() { }

        public virtual void Exit() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }
    }
}