using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Interfaces;
using Mirror;
using SceneManagement;
using UnityEngine;

namespace Managers
{
    public class NetworkLoadManager : Singleton<NetworkLoadManager>
    {
        private List<INetworkLoad> _networkLoaders = new();

        public override void OnStartServer()
        {
            NetworkServer.OnConnectedEvent += OnPlayerConnected;
            SceneTransition.OnClientTransitNewScene += OnClientTransitNewScene;
        }

        public override void OnStopServer()
        {
            NetworkServer.OnConnectedEvent -= OnPlayerConnected;
            SceneTransition.OnClientTransitNewScene -= OnClientTransitNewScene;
        }

        private void OnPlayerConnected(NetworkConnectionToClient conn) =>
            StartCoroutine(LoadRoutine(conn));

        private void OnClientTransitNewScene(NetworkConnectionToClient conn) => 
            StartCoroutine(LoadRoutine(conn));

        private IEnumerator LoadRoutine(NetworkConnectionToClient conn)
        {
            yield return new WaitUntil(() => conn.identity != null);

#if UNITY_EDITOR
            Debug.Log($"{conn.identity.name} is exist");
#endif
            if (!conn.identity.TryGetComponent(out EntityInitializer initializer))
            {
#if UNITY_EDITOR

                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
#endif
                yield break;
            }

            yield return new WaitUntil(() => initializer.IsInitialized);

#if UNITY_EDITOR
            Debug.Log($"{conn.identity.name} is initilaized. Check local player");
#endif
            if (!conn.identity.isLocalPlayer)
            {
#if UNITY_EDITOR

                Debug.Log($"{conn.identity.name}, start load data");
#endif
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
#if UNITY_EDITOR

                Debug.LogWarning("Loader is contains");
#endif
                return;
            }

            _networkLoaders.Add(loader);
        }

        public void RemoveLoader(INetworkLoad loader) =>
            _networkLoaders.Remove(loader);
    }
}