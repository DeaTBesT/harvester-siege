using Mirror;
using UnityEngine;
using Utils.ObjectPool;

namespace Managers
{
    public class PrefabPool : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _preloadCount;

        private GameObjectPool _objectPool;

        private void Start()
        {
            _objectPool = new GameObjectPool(_prefab, _preloadCount, transform);

            NetworkClient.RegisterPrefab(_prefab, SpawnHandler, UnspawnHandler);
        }

        private void OnDestroy() => 
            NetworkClient.UnregisterPrefab(_prefab);

        private GameObject SpawnHandler(SpawnMessage msg) =>
            _objectPool.Get();

        private void UnspawnHandler(GameObject spawned) =>
            _objectPool.Return(spawned);

        public GameObject Get() => 
            _objectPool.Get();

        public void Return(GameObject @object) => 
            _objectPool.Return(@object);
    }
}