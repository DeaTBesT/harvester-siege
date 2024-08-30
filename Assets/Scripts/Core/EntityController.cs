using Interfaces;
using Mirror;

namespace Core
{
    public abstract class EntityController : NetworkBehaviour, IInitialize
    {
        public virtual bool IsEnable { get; set; } = true;
        
        public abstract void Initialize(params object[] objects);
        
        public abstract void ActivateEntity();

        public abstract void DiactivateEntity();
    }
}