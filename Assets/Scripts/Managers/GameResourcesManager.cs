using System;
using System.Collections.Generic;
using System.Linq;
using GameResources.Core;
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
                    NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig));
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

            for (var i = 0; i < data.GameResourceNameList.Count; i++)
            {
                var resourceName = data.GameResourceNameList[i];
                var resourceAmount = data.GameResourceAmountList[i];

                var resourceConfig =
                    (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
                _resourcesData.Add(new ResourceData(resourceConfig, resourceAmount));
            }

            OnChangeResourcesData?.Invoke(_resourcesData);
        }

        public void AddResource(ResourceConfig resourceConfig, int amount = 1) =>
            AddResourceCmd(NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceConfig), amount);

        [Command(requiresAuthority = false)]
        private void AddResourceCmd(string resourceName, int amount) =>
            AddResourceRpc(resourceName, amount);

        [ClientRpc]
        private void AddResourceRpc(string resourceName, int amount)
        {
            var resourceConfig =
                (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
            var resourceData = _resourcesData.FirstOrDefault(x =>
                x.ResourceConfig.TypeResource == resourceConfig.TypeResource);

            if (resourceData != null)
            {
                resourceData.AddResource(amount);
                OnAddResource?.Invoke(resourceData);
            }
            else
            {
                _resourcesData.Add(new ResourceData(resourceConfig, amount));
                OnChangeResourcesData?.Invoke(_resourcesData);
            }
        }

        public void RemoveResource(ResourceConfig resourceConfig, int amount = 1) =>
            RemoveResourceCmd(NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceConfig), amount);

        [Command(requiresAuthority = false)]
        private void RemoveResourceCmd(string resourceName, int amount) =>
            RemoveResourceRpc(resourceName, amount);

        [ClientRpc]
        private void RemoveResourceRpc(string resourceName, int amount)
        {
            var resourceConfig =
                (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
            var resourceData = _resourcesData.FirstOrDefault(x =>
                x.ResourceConfig.TypeResource == resourceConfig.TypeResource);

            if (resourceData == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"ResourceData with type {resourceConfig.TypeResource} isn't exist");
#endif
                return;
            }

            if (resourceData.AmountResource - amount > 0)
            {
                resourceData.RemoveResource(amount);
                OnRemoveResource?.Invoke(resourceData);
            }
            else
            {
                _resourcesData.Remove(resourceData);
                OnChangeResourcesData?.Invoke(_resourcesData);
            }
        }
    }
}