using Interfaces;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace Core
{
    public abstract class EntityController : NetworkBehaviour, IInitialize
    {
        public virtual bool IsEnable { get; set; } = true;
        
        public abstract void Initialize(params object[] objects);
        
        public abstract void ActivateEntity();

        public abstract void DiactivateEntity();
        
        public abstract void ChangePosition(Vector2 newPosition);
    }
}