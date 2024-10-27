using System;
using GameResources.Core;
using GameResources.CustomSerialization;
using Interfaces;
using Managers;
using Mirror;
using Player;
using UnityEngine;

namespace GameResources
{
    public class GameResource : NetworkBehaviour, IInteractable, INetworkLoad
    {
        [SerializeField] private ResourceData _resourceData;

        public bool OneTimeInteract => true;
        public NetworkIdentity NetIdentity => netIdentity;

        public Action OnInteract { get; set; }
        public Action OnFinishInteract { get; set; }

        public override void OnStartServer() =>
            NetworkLoadManager.Instance.AddLoader(this);

        public override void OnStopServer() =>
            NetworkLoadManager.Instance.RemoveLoader(this);

        [Server]
        public void LoadDataServer(NetworkConnectionToClient conn)
        {
            var data = new GameResourceData(_resourceData.AmountResource);

            var writer = new NetworkWriter();
            writer.WriteGameResourceData(data);
            var writerData = writer.ToArray();

            LoadDataRpc(conn, writerData);
        }

        [TargetRpc]
        public void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData)
        {
            var reader = new NetworkReader(writerData);
            var data = reader.ReadGameResourceData();

            _resourceData.SetAmount(data.Amount);
        }

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

        public void SetAmount(int amount) =>
            SetAmountCmd(amount);

        [Command(requiresAuthority = false)]
        private void SetAmountCmd(int amount) =>
            SetAmountRpc(amount);

        [ClientRpc]
        private void SetAmountRpc(int amount) =>
            _resourceData.SetAmount(amount);
    }
}