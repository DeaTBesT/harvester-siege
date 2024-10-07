using System.Collections;
using System.Linq;
using Core;
using Mirror;
using PlayerModule;
using UnityEngine;

namespace Managers
{
    public class CustomNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn) =>
            StartCoroutine(ConnectingHandler(conn));

        private IEnumerator ConnectingHandler(NetworkConnectionToClient conn)
        {
            var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var playerName = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);

#if UNITY_EDITOR
            Debug.Log($"New player {conn.connectionId} connecting...");
#endif
            yield return new WaitUntil(() => conn.identity != null);

            if (!player.TryGetComponent(out PlayerInitializer initializer))
            {
#if UNITY_EDITOR
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
#endif
                yield break;
            }

            InitializeOtherPlayers(conn, out var isInitializedOtherPlayers);

            yield return new WaitUntil(() => isInitializedOtherPlayers);

            initializer.InitializeAllRpc(playerName);
            NetworkServer.SpawnObjects();

            yield return new WaitUntil(() => initializer.IsInitialized);

#if UNITY_EDITOR
            Debug.Log($"{conn.identity.name} is successfully initialized. Entering world...");
#endif
        }

        private void InitializeOtherPlayers(NetworkConnectionToClient conn, out bool isInitialized)
        {
            foreach (var player in NetworkServer.connections.Where(x => x.Value != conn))
            {
                var playerIdentity = player.Value.identity;

                if (playerIdentity == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("Identity is null");
#endif
                    continue;
                }

                if (!playerIdentity.TryGetComponent(out PlayerInitializer initializer))
                {
#if UNITY_EDITOR
                    Debug.LogError("Initializer is null");
#endif
                    continue;
                }

                var playerName = $"{playerPrefab.name} [connId={player.Value.connectionId}]";
                initializer.InitializeRpc(conn, playerName);
            }

            isInitialized = true;
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn) =>
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

            if (identity.isServer)
            {
#if UNITY_EDITOR
                Debug.Log($"{identity.name} is server");
#endif
                yield break;
            }

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

            initializer.Deinitialize();

            yield return new WaitUntil(() => !initializer.IsInitialized);

#if UNITY_EDITOR
            Debug.Log($"Player is deinitialized");
#endif
            NetworkServer.Destroy(identity.gameObject);
        }
    }
}