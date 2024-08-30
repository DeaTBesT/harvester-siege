#if UNITY_EDITOR
using Mirror;
using UnityEngine;
using ParrelSync;

public class MultiplayerTestScript : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;

    private void Start()
    {
        if (ClonesManager.IsClone())
        {
            _networkManager.StartClient();
        }
        else  
        { 
            _networkManager.StartHost();
        }
    }
}
#endif