using System.Collections.Generic;
using System.Linq;
using GameResources;
using GameResources.UI;
using Interfaces;
using Managers;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerUIController : NetworkBehaviour, IInitialize, IDeinitialize
    {
        [SerializeField] private GameObject _canvas;
        
        [SerializeField] private Transform _resourcesParent;
        [SerializeField] private ResourceDataUI _resourcePrefab;

        private List<ResourceDataUI> _resourcesDataUI = new();

        private GameResourcesManager _resourcesManager;

        public bool IsEnable { get; set; }

        public void Initialize(params object[] objects)
        {
            if (!isLocalPlayer)
            {
                _canvas.SetActive(false);
            }
            
            _resourcesManager = objects[0] as GameResourcesManager;

            if ((_resourcesManager != null) && (isLocalPlayer))
            {
                _resourcesManager.OnAddResource += OnAddResource;
                _resourcesManager.OnChangeResourcesData += OnChangeResources;
                _resourcesManager.OnRemoveResource += OnRemoveResource;
            }
        }

        public void Deinitialize()
        {
            if (_resourcesManager != null)
            {
                _resourcesManager.OnAddResource -= OnAddResource;
                _resourcesManager.OnChangeResourcesData -= OnChangeResources;
                _resourcesManager.OnRemoveResource -= OnRemoveResource;
            }
        }

        private void OnAddResource(ResourceData data)
        {
            var resourceDataUI = _resourcesDataUI.FirstOrDefault(x => x.ResourceDataConfig == data);

            if (resourceDataUI != null)
            {
                resourceDataUI.UpdateResource();
                return;
            }

#if UNITY_EDITOR
            Debug.LogError("None resource data UI");
#endif
        }

        private void OnChangeResources(List<ResourceData> dataList) => 
            GenerateResourcesPanel(dataList);

        private void OnRemoveResource(ResourceData data)
        {
            var resourceDataUI = _resourcesDataUI.FirstOrDefault(x => x.ResourceDataConfig == data);

            if (resourceDataUI != null)
            {
                _resourcesDataUI.Remove(resourceDataUI);
                Destroy(resourceDataUI.gameObject);
                return;
            }

#if UNITY_EDITOR
            Debug.LogError("None resource data UI");
#endif
        }

        private void GenerateResourcesPanel(List<ResourceData> dataList)
        {
            ClearResourcesPanel();

            foreach (var data in dataList)
            {
                var resourceDataUI = Instantiate(_resourcePrefab, _resourcesParent);
                resourceDataUI.ChangeResourceData(data);
                _resourcesDataUI.Add(resourceDataUI);
            }
        }

        private void ClearResourcesPanel()
        {
            foreach (Transform child in _resourcesParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}