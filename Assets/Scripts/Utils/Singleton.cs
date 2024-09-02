using Mirror;
using UnityEngine;

public class Singleton<T>  : NetworkBehaviour where T:Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance)
            {
                _instance = FindObjectOfType<T>();

                if (_instance is null)
                {
                    GameObject newInstance = new GameObject("GameManager");
                    _instance = newInstance.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != null)
        {
            Destroy(gameObject);
        }
    }
}
