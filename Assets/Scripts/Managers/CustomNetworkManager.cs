using System;
using System.Collections;
using System.Linq;
using Core;
using Mirror;
using PlayerModule;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class CustomNetworkManager : NetworkManager
    {
        [Scene, SerializeField] private string[] _scenesToLoad;

        private bool _isStartSceneLoaded;
        private bool _isSubSceneLoaded;
        private bool _isTransition;

        private Coroutine _serverLoadSubScenesRoutine;
        private Coroutine _loadAdditiveSceneRoutine;
        private Coroutine _unloadAdditiveSceneRoutine;

        public Action<string> OnClientChangedScene { get; set; }

        #region OnChangingScenes

        public override void OnServerSceneChanged(string newSceneName)
        {
            base.OnServerSceneChanged(newSceneName);

            if (newSceneName == onlineScene)
            {
                ServerLoadSubScenes();
            }
        }

        private void ServerLoadSubScenes()
        {
            if (_serverLoadSubScenesRoutine != null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("You try to load sub scenes when they loading now");
#endif
                return;
            }

            _serverLoadSubScenesRoutine = StartCoroutine(ServerLoadSubScenesRoutine());
        }

        private IEnumerator ServerLoadSubScenesRoutine()
        {
            foreach (var additiveScene in _scenesToLoad)
            {
                yield return SceneManager.LoadSceneAsync(additiveScene, new LoadSceneParameters
                {
                    loadSceneMode = LoadSceneMode.Additive,
                    localPhysicsMode = LocalPhysicsMode.Physics2D
                });
            }

            _isSubSceneLoaded = true;
            _serverLoadSubScenesRoutine = null;
        }

        public override void OnClientSceneChanged()
        {
            if (!_isTransition)
            {
                base.OnClientSceneChanged();
            }
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation,
            bool customHandling)
        {
            switch (sceneOperation)
            {
                case SceneOperation.LoadAdditive:
                {
                    LoadAdditive(newSceneName);
                }
                    break;
                case SceneOperation.UnloadAdditive:
                {
                    UnloadAdditive(newSceneName);
                }
                    break;
            }
        }

        private void LoadAdditive(string sceneName)
        {
            if (_loadAdditiveSceneRoutine != null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("You try to load additive scene when they loading now");
#endif
                return;
            }

            _loadAdditiveSceneRoutine = StartCoroutine(LoadAdditiveRoutine(sceneName));
        }

        private IEnumerator LoadAdditiveRoutine(string sceneName)
        {
            _isTransition = true;

            //Затемнение

            if (mode == NetworkManagerMode.ClientOnly)
            {
                loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                yield return new WaitUntil(() => loadingSceneAsync is { isDone: false });
            }

            NetworkClient.isLoadingScene = false;
            _isTransition = false;

            //SceneManager.SetActiveScene(SceneManager.GetSceneByPath(sceneName));
            OnClientSceneChanged();
            OnClientChangedScene?.Invoke(sceneName);

            _isStartSceneLoaded = true;

            yield return new WaitUntil(() => _isStartSceneLoaded);

            //Убрать затемнение
            _loadAdditiveSceneRoutine = null;
        }

        private void UnloadAdditive(string sceneName)
        {
            if (_unloadAdditiveSceneRoutine != null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("You try to unload additive scene when they unloading now");
#endif
                return;
            }

            _unloadAdditiveSceneRoutine = StartCoroutine(UnloadAdditiveRoutine(sceneName));
        }

        private IEnumerator UnloadAdditiveRoutine(string sceneName)
        {
            _isTransition = true;

            //Затемнение

            if (mode == NetworkManagerMode.ClientOnly)
            {
                yield return SceneManager.UnloadSceneAsync(sceneName);
                yield return Resources.UnloadUnusedAssets();
            }

            NetworkClient.isLoadingScene = false;
            _isTransition = false;

            OnClientSceneChanged();

            //Убрать затемнение
            _unloadAdditiveSceneRoutine = null;
        }

        #endregion

        #region OnPlayerConnected

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);

            if (conn.identity == null)
            {
                ConnectPlayer(conn);
            }
        }

        private void ConnectPlayer(NetworkConnectionToClient conn) =>
            StartCoroutine(ConnectingHandler(conn));

        private IEnumerator ConnectingHandler(NetworkConnectionToClient conn)
        {
            yield return new WaitUntil(() => _isSubSceneLoaded);

            conn.Send(new SceneMessage
                { sceneName = _scenesToLoad[0], sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

            yield return new WaitForEndOfFrame();

            var startPosition = GetStartPosition();
            var player = Instantiate(playerPrefab, startPosition);
            player.transform.parent = null;
            var playerName = $"{playerPrefab.name} [connId={conn.connectionId}]";

            yield return new WaitForEndOfFrame();

            NetworkServer.AddPlayerForConnection(conn, player);

            yield return new WaitUntil(() => conn.identity != null);

            if (!player.TryGetComponent(out PlayerInitializer initializer))
            {
#if UNITY_EDITOR
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
#endif
                yield break;
            }

            initializer.InitializeAllRpc(playerName);

            yield return new WaitUntil(() => initializer.IsInitialized);

#if UNITY_EDITOR
            Debug.Log($"{conn.identity.name} is successfully initialized. Entering world...");
#endif
        }

        #endregion

        #region OnPlayerDisconnected

        public override void OnServerDisconnect(NetworkConnectionToClient conn) =>
            DisconnectPlayer(conn);

        private void DisconnectPlayer(NetworkConnectionToClient conn) =>
            StartCoroutine(DisconnectingHandler(conn));

        private IEnumerator DisconnectingHandler(NetworkConnectionToClient conn)
        {
            if (NetworkServer.connections.Count <= 0)
            {
                yield break;
            }

            var serverConn = NetworkServer.connections.ToDictionary(x =>
                x.Value.identity.isServer).First().Value.Value;

#if UNITY_EDITOR
            Debug.Log($"{conn.identity.name} disconnecting...");
#endif

            var identity = conn.identity;

            NetworkServer.RemovePlayerForConnection(conn);
            identity.AssignClientAuthority(serverConn);

            yield return new WaitUntil(() => identity.isOwned);

            if (!identity.TryGetComponent(out EntityInitializer initializer))
            {
#if UNITY_EDITOR
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
#endif
                yield break;
            }

            initializer.Quit();

            yield return new WaitUntil(() => !initializer.IsInitialized);

#if UNITY_EDITOR
            Debug.Log($"Player is deinitialized");
#endif
            NetworkServer.Destroy(identity.gameObject);
        }

        #endregion
    }
}