using Managers;
using Mirror;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class ManagersMover : NetworkBehaviour
    {
        private CustomNetworkManager _customNetworkManager;

        public override void OnStartServer()
        {
            if (!isServer)
            {
                return;
            }

            _customNetworkManager = (CustomNetworkManager)NetworkManager.singleton;

            _customNetworkManager.OnClientChangedScene += OnClientChangeScene;
        }

        public override void OnStopServer() => 
            _customNetworkManager.OnClientChangedScene -= OnClientChangeScene;

        private void OnClientChangeScene(string sceneName)
        {
            transform.parent = null;
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByPath(sceneName));
        }
    }
}