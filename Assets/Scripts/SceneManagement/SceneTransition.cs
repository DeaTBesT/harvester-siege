using System;
using System.Collections;
using Managers;
using Mirror;
using PlayerModule;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneTransition : NetworkBehaviour
    {
        [Scene, SerializeField] private string _transitionScene;
        [SerializeField] private Vector3 _startPosition = Vector3.zero;

        private CustomNetworkManager _customNetworkManager;

        private Coroutine _transitionRoutine;

        public static Action<NetworkConnectionToClient> OnClientTransitNewScene { get; set; }
        
        private void Awake()
        {
            if (_customNetworkManager == null)
            {
                _customNetworkManager = (CustomNetworkManager)NetworkManager.singleton;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerInitializer _))
            {
                Debug.Log($"Player initializer is null: {other.name}");
                return;
            }

            if (!isServer)
            {
                return;
            }

            TransitionPlayerNewScene(other.gameObject);
        }
        
        [ServerCallback]
        private void TransitionPlayerNewScene(GameObject player)
        {
            if (_transitionRoutine != null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Player transitioning already");
#endif
                return;
            }

#if UNITY_EDITOR
            Debug.Log($"Start transition: {player.name}");
#endif
            _transitionRoutine = StartCoroutine(TransitionPlayerNewSceneRoutine(player));
        }

        private IEnumerator TransitionPlayerNewSceneRoutine(GameObject player)
        {
            if (!player.TryGetComponent(out NetworkIdentity identity))
            {
                yield break;
            }

            var conn = identity.connectionToClient;

            if (conn == null)
            {
                yield break;
            }

            if (!player.TryGetComponent(out PlayerInitializer initializer))
            {
#if UNITY_EDITOR
                Debug.LogError($"Entity initializer is null: {conn.connectionId}");
#endif
                yield break;
            }

            //initializer.DeinitializeAllRpc();
            initializer.DeinitializeLocal();
            
            //yield return new WaitUntil(() => !initializer.IsInitialized);
            yield return new WaitForEndOfFrame();

            //Затемнение

            conn.Send(new SceneMessage
            {
                sceneName = gameObject.scene.path, sceneOperation = SceneOperation.UnloadAdditive,
                customHandling = true
            });
            
            NetworkServer.RemovePlayerForConnection(conn, RemovePlayerOptions.Unspawn);

            // _customNetworkManager.DeinitializeOtherPlayers(conn, out var isInitialized);
            //
            // yield return new WaitUntil(() => !isInitialized);
            
            player.transform.position = _startPosition;
            player.transform.rotation = Quaternion.identity;

            SceneManager.MoveGameObjectToScene(identity.gameObject, SceneManager.GetSceneByPath(_transitionScene));

            conn.Send(new SceneMessage
            {
                sceneName = _transitionScene, sceneOperation = SceneOperation.LoadAdditive,
                customHandling = true
            });

            var playerName = $"Player [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);

            yield return new WaitUntil(() => conn.identity != null);
            
            //initializer.InitializeAllRpc(playerName);
            initializer.InitializeLocal(playerName);
            
            yield return new WaitForEndOfFrame();

            OnClientTransitNewScene?.Invoke(conn);
            
            // yield return new WaitUntil(() =>
            // {
            //     Debug.Log("Initialize: " + initializer.IsInitialized);
            //     return initializer.IsInitialized;
            // });

#if UNITY_EDITOR
            Debug.Log($"{conn.identity.name} is successfully transition");
#endif
            _transitionRoutine = null;
        }
    }
}