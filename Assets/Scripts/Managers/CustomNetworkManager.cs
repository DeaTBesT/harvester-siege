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
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
            
            Debug.Log($"New player {conn.connectionId} connecting...");
        
            yield return new WaitUntil(() => conn.identity != null);

            if (!player.TryGetComponent(out PlayerInitializer initializer))
            {
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
            }
            
            InitializeOtherPlayers(conn, out var isInitializedOtherPlayers);

            yield return new WaitUntil(() => isInitializedOtherPlayers);
            
            initializer.InitializeRpc();
            
            yield return new WaitUntil(() => initializer.IsInitialized);
        
            Debug.Log($"{conn.identity.name} is initialized");
        }
        
        private void InitializeOtherPlayers(NetworkConnectionToClient conn, out bool isInitialized)
        {
            foreach (var player in NetworkServer.connections.Where(x => x.Value != conn))
            {
                var playerIdentity = player.Value.identity;
                
                if (playerIdentity == null)
                {
                    Debug.LogError("Identity is null");
                    continue;
                }

                if (!playerIdentity.TryGetComponent(out PlayerInitializer initializer))
                {
                    Debug.LogError("Initializer is null");
                    continue;
                }
                //
                // if (initializer.IsInitialized)
                // {
                //     Debug.LogError($"Player {playerIdentity.name} is initialized");
                //     continue;
                // }
                
                initializer.InitializeRpc();
            }

            isInitialized = true;
        }
        
        public override void OnServerDisconnect(NetworkConnectionToClient conn) =>
            StartCoroutine(DisconnectingHandler(conn));

        private IEnumerator DisconnectingHandler(NetworkConnectionToClient conn)
        {
            var serverConn = NetworkServer.connections.ToDictionary(x =>
                x.Value.identity.isServer).First().Value.Value;

            Debug.Log($"{conn.identity.name} disconnecting...");

            var identity = conn.identity;

            NetworkServer.RemovePlayerForConnection(conn);
            identity.AssignClientAuthority(serverConn);

            yield return new WaitUntil(() => identity.isOwned);

            if (!identity.TryGetComponent(out EntityInitializer initializer))
            {
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
            }

            initializer.Deinitialize();

            yield return new WaitUntil(() => !initializer.IsInitialized);

            Debug.Log($"Player is deinitialized");

            NetworkServer.Destroy(identity.gameObject);
        }
    }
}