using Core;
using GameResources.Core;
using UnityEngine;

namespace GameResources.ResourceVein
{
    public class ResourceVeinInitializer : EntityInitializer
    {
        [SerializeField] private ResourceVeinStats _resourceVeinStats;
        [SerializeField] private ResourceSpawner _resourceSpawner;

        protected override void OnValidate()
        {
            if (_resourceVeinStats == null)
            {
                _resourceVeinStats = GetComponent<ResourceVeinStats>();
            }
            
            if (_resourceSpawner == null)
            {
                _resourceSpawner = GetComponent<ResourceSpawner>();
            }
        }

        public override void Initialize() => 
            _resourceVeinStats.Initialize(_resourceSpawner);
    }
}