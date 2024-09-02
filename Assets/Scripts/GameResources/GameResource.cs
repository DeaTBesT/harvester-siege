using System;
using Interfaces;
using Mirror;
using Player;
using UnityEngine;

namespace GameResources
{
    public class GameResource : NetworkBehaviour, IInteractable
    {
        [SerializeField] private ResourceData _resourceData;
        
        public bool OneTimeInteract => true;
        
        public Action OnInteract { get; set; }
        public Action OnFinishInteract { get; set; }
        
        public bool TryInteract(IInteractor interactor)
        {
            var interactableNetId = interactor.InteractableNetId;

            if (interactableNetId.TryGetComponent(out PlayerInventoryController playerInventory))
            {
                playerInventory.AddResource(_resourceData);
                NetworkServer.Destroy(gameObject);
                return true;
            }
            
            return false;
        }

        public void FinishInteract(IInteractor interactor)
        {
            throw new NotImplementedException();
        }
    }
}