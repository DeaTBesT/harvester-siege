#if UNITY_EDITOR
using Mirror;
using UnityEngine;
using ParrelSync;

public class MultiplayerTestController : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;

    private void OnValidate()
    {
        if (_networkManager == null)
        {
            _networkManager = FindObjectOfType<NetworkManager>();
        }
    }

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