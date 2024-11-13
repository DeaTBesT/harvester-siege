using System;
using Interfaces;
using Mirror;
using UnityEngine;

namespace Core
{
    public abstract class EntityController : NetworkBehaviour, IInitialize
    {
        public virtual bool IsEnable { get; set; } = true;
        
        public abstract void Initialize(params object[] objects);
        
        public abstract void ActivateEntity();

        public abstract void DiactivateEntity();
        
        public abstract void ChangePosition(Vector2 newPosition);

        public virtual void ActivateMoveEntity() => 
            throw new NotImplementedException();

        public virtual void DiactivateMoveEntity() => 
            throw new NotImplementedException();
    }
}