using Interfaces;
using Mirror;

namespace Core
{
    public abstract class EntityStats : NetworkBehaviour, IInitialize
    {
        protected float _health;
        
        public abstract int TeamId { get; }

        public virtual bool IsEnable { get; set; } = true;
        
        public virtual void Initialize(params object[] objects)
        {
            
        }
        
        public abstract void TakeDamage(int teamId, float amount);
    }
}