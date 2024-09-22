using Mirror;
using UnityEngine;

namespace Core
{
    public abstract class EntityInitializer : NetworkBehaviour
    {
        [SerializeField] private bool _selfInitialize = true;
        
        public bool IsInitialized { get; protected set; }

        public override void OnStartClient()
        {
            if (!_selfInitialize)
            {
                return;
            }
            
            Initialize();
        }

        public override void OnStopClient()
        {
            if (!_selfInitialize)
            {
                return;
            }
            
            Deinitialize();
        }

        public abstract void Initialize();

        public virtual void Deinitialize()
        {
        }
    }
}