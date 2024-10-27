using GameResources.Core;
using Interfaces;
using Mirror;

namespace Core
{
    public class EntityInventoryController : NetworkBehaviour, IInitialize, IDeinitialize
    {
        public bool IsEnable { get; set; }
        
        public virtual void Initialize(params object[] objects)
        {
            
        }

        public virtual void Deinitialize()
        {
            throw new System.NotImplementedException();
        }
        
        public virtual void AddResource(ResourceData resourceData)
        {
            
        }
        
        public virtual void DropResource(ResourceData resourceData)
        {
            
        }

        public virtual void DropAllResources()
        {
            
        }
    }
}