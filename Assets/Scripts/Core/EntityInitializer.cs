using System.Collections;
using Mirror;
using UnityEngine;

namespace Core
{
    public abstract class EntityInitializer : NetworkBehaviour
    {
        [SerializeField] private bool _selfInitialize = true;
        [SerializeField] private bool _selfDeinitialize = true;

        [SerializeField] private GameObject _entityObjectContainerPrefab;
        
        protected Transform _entityObjectContainer;
        
        public bool IsInitialized { get; protected set; }

        public override void OnStartClient()
        {
            CreateEntityObjectContainer();
            
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

        public abstract void Initialize();

        public virtual void Deinitialize()
        {
            if (_entityObjectContainer != null)
            {
                NetworkServer.Destroy(_entityObjectContainer.gameObject);
            }
        }

        private void CreateEntityObjectContainer()
        {
            CreateEntityObjectContainerCmd();
        }
        
        [Command]
        private void CreateEntityObjectContainerCmd() => 
            StartCoroutine(CreateEntityObjectContainerHandler());

        private IEnumerator CreateEntityObjectContainerHandler()
        {
            while (isOwned)
            {
                yield return null;
            }
            
            _entityObjectContainer = Instantiate(_entityObjectContainerPrefab, Vector3.zero, Quaternion.identity).transform;
            _entityObjectContainer.name = gameObject.name + " object container";
            
            NetworkServer.Spawn(_entityObjectContainer.gameObject, connectionToClient);
        } 
    }
}