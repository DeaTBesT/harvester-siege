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
                DestroySelfCmd();
                return true;
            }

            return false;
        }

        public void FinishInteract(IInteractor interactor)
        {
            throw new NotImplementedException();
        }

        public void ForceFinishInteract(IInteractor interactor)
        {
            throw new NotImplementedException();
        }

        [Command(requiresAuthority = false)]
        private void DestroySelfCmd() =>
            NetworkServer.Destroy(gameObject);
    }
}