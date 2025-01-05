using System.Collections.Generic;
using GameResources.Core;
using Mirror;
using Player;
using Player.CustomSerialization;
using UnityEngine;
using Utils.Networking;

namespace Managers
{
    public class NetworkScenesProvider : Singleton<NetworkScenesProvider>
    {
        public void SendPlayerInventoryData(NetworkIdentity identity, List<ResourceData> resourcesData)
        {
            var resourceNameList = new List<string>();
            var resourceAmountList = new List<int>();

            foreach (var resourceData in resourcesData)
            {
                resourceNameList.Add(
                    NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig));
                resourceAmountList.Add(resourceData.AmountResource);
            }

            var data = new PlayerInventoryControllerData(resourceNameList, resourceAmountList);

            var writer = new NetworkWriter();
            writer.WritePlayerInventoryControllerData(data);
            var writerData = writer.ToArray();

            SendPlayerInventoryDataCmd(identity, writerData);
        }

        [Command(requiresAuthority = false)]
        private void SendPlayerInventoryDataCmd(NetworkIdentity identity, byte[] writerData) => 
            SendPlayerInventoryDataRpc(identity, writerData);

        [ClientRpc(includeOwner = false)]
        private void SendPlayerInventoryDataRpc(NetworkIdentity identity, byte[] writerData)
        {
            if (!identity.TryGetComponent(out PlayerInventoryController inventoryController))
            {
                return;
            }
            
            Debug.Log($"Send player inventory some data: {identity.name} : {writerData.Length}");
            inventoryController.UpdateData(writerData);
        }
    }
}