using System.Collections.Generic;
using System.Linq;
using Enums;
using Interfaces;
using UnityEngine;

namespace Managers
{
    public class PrefabPoolManager : Singleton<PrefabPoolManager>
    {
        [SerializeField] private List<PrefabPoolConfig> _poolConfigs = new();

        [System.Serializable]
        public class PrefabPoolConfig
        {
            public PrefabPool prefabsPool;
            public PoolType poolType;
        }

        public bool IsEnable { get; set; }

        public PrefabPool GetPool(PoolType poolType)
        {
            var pool = _poolConfigs.FirstOrDefault(x => x.poolType == poolType);

            if (pool == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"Pool isn't exist: {poolType}");
#endif
            }

            return pool.prefabsPool;
        }
    }
}