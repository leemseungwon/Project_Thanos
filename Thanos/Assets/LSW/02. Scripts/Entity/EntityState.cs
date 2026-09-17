using System;

namespace LSW._02._Scripts.Entity
{
    public abstract class EntityState
    {
        protected readonly BaseEntity Entity;
        protected readonly EntityAnimation Animation;
        
        protected Action EndAction;

        protected EntityState(BaseEntity entity)
        {
            Entity = entity;
            Animation = entity.Animation;
        }

        public virtual void Enter(Action endAction = null)
        {
            if (endAction != null)
            {
                EndAction = endAction;
            }
        }

        public virtual void Exit()
        {
            EndAction?.Invoke();
            EndAction = null;
        }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }
        
        public virtual void Dispose() { }
    }
}