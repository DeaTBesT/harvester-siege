using System.Collections;
using System.Linq;
using Core;
using Mirror;
using UnityEngine;

namespace Managers
{
    public class CustomNetworkManager : NetworkManager
    {
        // public override void OnServerConnect(NetworkConnectionToClient conn)
        // {
        //     base.OnServerConnect(conn);
        // }
        //
        // public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        // {
        //     StartCoroutine(ConnectingHandler(conn));
        // }
        //
        // private IEnumerator ConnectingHandler(NetworkConnectionToClient conn)
        // {
        //     GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //     player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        //     NetworkServer.AddPlayerForConnection(conn, player);
        //     
        //     Debug.Log($"New player {conn.connectionId} connecting...");
        //
        //     while (conn.identity == null)
        //     {
        //         yield return null;
        //     }
        //
        //     if (!player.TryGetComponent(out EntityInitializer initializer))
        //     {
        //         Debug.LogError($"Entity initializer is null: {conn.connectionId}");
        //     }
        //
        //     initializer.Initialize();
        //     
        //     while (!initializer.IsInitialized)
        //     {
        //         yield return null;
        //     }
        //
        //     //NetworkServer.Spawn(player);
        //     
        //     Debug.Log($"{conn.identity.name} is initialized");
        //
        //     // NetworkServer.DestroyPlayerForConnection(conn);
        // }

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

            while (!identity.isOwned)
            {
                Debug.LogWarning("Wait owned object");
                yield return null;
            }

            if (!identity.TryGetComponent(out EntityInitializer initializer))
            {
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
            }

            initializer.Deinitialize();

            while (initializer.IsInitialized)
            {
                yield return null;
            }

            Debug.Log($"Player is deinitialized");

            NetworkServer.Destroy(identity.gameObject);
        }
    }
}