using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Interfaces;
using Mirror;
using UnityEngine;

namespace Managers
{
    public class NetworkLoadManager : Singleton<NetworkLoadManager>
    {
        private List<INetworkLoad> _networkLoaders = new();

        public override void OnStartServer() =>
            NetworkServer.OnConnectedEvent += OnPlayerConnected;

        public override void OnStopServer() => 
            NetworkServer.OnConnectedEvent -= OnPlayerConnected;

        private void OnPlayerConnected(NetworkConnectionToClient conn) => 
            StartCoroutine(LoadRoutine(conn));

        private IEnumerator LoadRoutine(NetworkConnectionToClient conn)
        {
            yield return new WaitUntil(() => conn.identity != null);

            Debug.Log($"{conn.identity.name} is exist");
            
            if (!conn.identity.TryGetComponent(out EntityInitializer initializer))
            {
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
            }
            
            yield return new WaitUntil(() => initializer.IsInitialized);
            
            Debug.Log($"{conn.identity.name} is initilaized. Check local player");
            
            if (!conn.identity.isLocalPlayer)
            {
                Debug.Log($"{conn.identity.name}, start load data");
                LoadAllData(conn);
            }
        }

        private void LoadAllData(NetworkConnectionToClient target)
        {
            foreach (var loader in _networkLoaders.Where(loader => loader != null))
            {
                loader.LoadDataServer(target);
            }
        }

        public void AddLoader(INetworkLoad loader)
        {
            var isContains = _networkLoaders.Contains(loader);

            if (isContains)
            {
                Debug.LogWarning("Loader is contains");
                return;
            }

            _networkLoaders.Add(loader);
        }

        public void RemoveLoader(INetworkLoad loader) =>
            _networkLoaders.Remove(loader);
    }
}