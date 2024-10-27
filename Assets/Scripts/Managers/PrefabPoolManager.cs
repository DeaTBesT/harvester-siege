using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Managers
{
    public class PrefabPoolManager : Singleton<PrefabPoolManager>
    {
        [SerializeField] private List<PrefabPool> _prefabPools = new();

        protected override void OnValidate()
        {
            if (transform.childCount != _prefabPools.Count)
            {
                _prefabPools.Clear();

                foreach (Transform child in transform)
                {
                    if (child.TryGetComponent(out PrefabPool prefabPool))
                    {
                        _prefabPools.Add(prefabPool);
                    }
                    else
                    {
                        Debug.LogWarning($"{child.name} isn't prefab pool");
                    }
                }
            }
        }

        public PrefabPool GetPool(PoolType poolType)
        {
            var pool = _prefabPools.FirstOrDefault(x => x.TypePool == poolType);

            if (pool == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"Pool isn't exist: {poolType}");
#endif
            }

            return pool;
        }
    }
}