using GameResources;
using Interfaces;
using Mirror;

namespace Core
{
    public class EntityInventoryController : NetworkBehaviour, IInitialize
    {
        public bool IsEnable { get; set; }
        
        public virtual void Initialize(params object[] objects)
        {
            
        }

        public virtual void AddResource(ResourceData resourceData)
        {
            
        }
        
        public virtual void RemoveResource(ResourceData resourceData)
        {
            
        }
    }
}