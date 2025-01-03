using Interfaces;
using Mirror;
using UnityEngine;

namespace Core
{
    public abstract class EntityInitializer : NetworkBehaviour, IInitialize, IDeinitialize
    {
        [SerializeField] private bool _selfInitialize = true;
        [SerializeField] private bool _selfDeinitialize = true;
        
        public bool IsInitialized { get; protected set; }

        public bool IsEnable { get; set; } = true;
        
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
            if (!_selfDeinitialize)
            {
                return;
            }
            
            Deinitialize();
        }

        public abstract void Initialize(params object[] objects);

        public virtual void Deinitialize(params object[] objects)
        {
          
        }

        public virtual void Quit()
        {
            
        }
    }
}