using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Managers
{
    public class PrefabPoolManager : Singleton<PrefabPoolManager>
    {
        [SerializeField] private List<PrefabPool> _prefabPools = new();

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