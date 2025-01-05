using Enums;
using Mirror;
using UnityEngine;
using Utils.ObjectPool;

namespace Managers
{
    public class PrefabPool : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _preloadCount;
        [SerializeField] private PoolType _poolType;
        
        private GameObjectPool _objectPool;

        public PoolType TypePool => _poolType;

        private void OnValidate()
        {
            var str = _prefab == null ? "Prefab pool" : $"Prefab pool: {_prefab.name}";
            gameObject.name = str;
        }

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

        public GameObject Get(Transform owner)
        {
            var @object = _objectPool.Get();
            @object.transform.SetParent(owner.transform);
            @object.transform.parent = null;

            return @object;
        }

        public void Return(GameObject @object) => 
            _objectPool.Return(@object);
    }
}