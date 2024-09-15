using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameResources;
using Interfaces;
using Managers.CustomSerialization;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace Managers
{
    public class GameResourcesManager : Singleton<GameResourcesManager>, INetworkLoad
    {
        private List<ResourceData> _resourcesData = new();

        public Action<ResourceData> OnAddResource { get; set; }
        public Action<ResourceData> OnRemoveResource { get; set; }
        public Action<List<ResourceData>> OnChangeResourcesData { get; set; }

        public override void OnStartServer() => 
            NetworkLoadManager.Instance.AddLoader(this);

        public override void OnStopServer() => 
            NetworkLoadManager.Instance.RemoveLoader(this);

        [Server]
        public void LoadDataServer(NetworkConnectionToClient conn)
        {
            var resourceNameList = new List<string>();
            var resourceAmountList = new List<int>();

            foreach (var resourceData in _resourcesData)
            {
                resourceNameList.Add(
                    NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.gameResourceDataConfig));
                resourceAmountList.Add(resourceData.AmountResource);
            }

            var data = new GameResourcesManagerData(resourceNameList, resourceAmountList);

            var writer = new NetworkWriter();
            writer.WriteGameResourcesManagerData(data);
            var writerData = writer.ToArray();

            LoadDataRpc(conn, writerData);
        }

        [TargetRpc]
        public void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData)
        {
            var reader = new NetworkReader(writerData);
            var data = reader.ReadGameResourcesManagerData();

            for (int i = 0; i < data.GameResourceNameList.Count; i++)
            {
                var resourceName = data.GameResourceNameList[i];
                var resourceAmount = data.GameResourceAmountList[i];

                var gameResource =
                    (GameResourceData)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
                _resourcesData.Add(new ResourceData(gameResource, resourceAmount));
            }

            OnChangeResourcesData?.Invoke(_resourcesData);
        }

        public void AddResource(GameResourceData gameResourceData, int amount = 1) =>
            AddResourceCmd(NetworkScriptableObjectSerializer.SerializeScriptableObject(gameResourceData), amount);

        [Command(requiresAuthority = false)]
        private void AddResourceCmd(string gameResourceName, int amount) =>
            AddResourceRpc(gameResourceName, amount);

        [ClientRpc]
        private void AddResourceRpc(string gameResourceName, int amount)
        {
            var gameResource =
                (GameResourceData)NetworkScriptableObjectSerializer.DeserializeScriptableObject(gameResourceName);
            var resourceData = _resourcesData.FirstOrDefault(x =>
                x.gameResourceDataConfig.TypeGameResource == gameResource.TypeGameResource);

            if (resourceData != null)
            {
                resourceData.AddResource(amount);
                OnAddResource?.Invoke(resourceData);
            }
            else
            {
                _resourcesData.Add(new ResourceData(gameResource, amount));
                OnChangeResourcesData?.Invoke(_resourcesData);
            }
        }

        public void RemoveResource(GameResourceData gameResourceData, int amount = 1)
        {
            RemoveResourceServer(NetworkScriptableObjectSerializer.SerializeScriptableObject(gameResourceData), amount);
        }

        [Server]
        private void RemoveResourceServer(string gameResourceName, int amount) =>
            RemoveResourceRpc(gameResourceName, amount);

        [ClientRpc]
        private void RemoveResourceRpc(string gameResourceName, int amount)
        {
        }
    }
}