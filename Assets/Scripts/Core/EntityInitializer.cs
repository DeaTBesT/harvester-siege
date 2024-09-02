using Mirror;

namespace Core
{
    public abstract class EntityInitializer : NetworkBehaviour
    {
        public override void OnStartClient() => 
            Initialize();

        public override void OnStopClient() => 
            Deinitialize();

        protected abstract void Initialize();

        protected virtual void Deinitialize()
        {
            
        }
    }
}