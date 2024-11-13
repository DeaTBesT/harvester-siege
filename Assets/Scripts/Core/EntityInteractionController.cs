using Interfaces;
using Mirror;
using UnityEngine;

namespace Core
{
    public abstract class EntityInteractionController : NetworkBehaviour, IInitialize, IDeinitialize, IInteractor
    {
        protected IInput _inputModule;

        private bool _isInteract = true;
        
        public NetworkIdentity InteractableNetId => netIdentity;
        
        public virtual bool IsEnable { get; set; } = true;
        
        public virtual void Initialize(params object[] objects)
        {
            _inputModule = objects[0] as IInput;

            if (_inputModule != null)
            {
                _inputModule.OnInteract += OnInteract;
                _inputModule.OnInteractUp += OnInteractUp;
            }
            else
            {
                Debug.LogError("Input module is null");
            }
        }

        public virtual void Deinitialize()
        {
            if (_inputModule != null)
            {
                _inputModule.OnInteract = null;
            }
        }

        public abstract void OnInteract();

        public virtual void OnInteractUp()
        {
            
        }

        public abstract void OnEndInteract();

        protected virtual void ToggleInteractionEvents()
        {
            _isInteract = !_isInteract;

            if (_isInteract)
            {
                _inputModule.OnInteract += OnInteract;
                _inputModule.OnInteract -= OnEndInteract;
            }
            else
            {
                _inputModule.OnInteract -= OnInteract;
                _inputModule.OnInteract += OnEndInteract;
            }
        }
    }
}