using System.Collections.Generic;
using System.Linq;
using GameResources.Core;
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

        [Header("Resources data")] [SerializeField]
        private Transform _resourcesParent;

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

            if (_resourcesManager == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Resources Manager is null");
#endif
                return;
            }

            if (!isLocalPlayer)
            {
                return;
            }
            
            _resourcesManager.OnAddResource += OnAddResource;
            _resourcesManager.OnChangeResourcesData += OnChangeResources;
            _resourcesManager.OnRemoveResource += OnRemoveResource;
        }

        public void Deinitialize(params object[] objects)
        {
            if (_resourcesManager == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Resources Manager is null");
#endif
                return;
            }

            if (!isLocalPlayer)
            {
                return;
            }
            
            _resourcesManager.OnAddResource -= OnAddResource;
            _resourcesManager.OnChangeResourcesData -= OnChangeResources;
            _resourcesManager.OnRemoveResource -= OnRemoveResource;
        }

        private void OnAddResource(ResourceData data)
        {
            var resourceDataUI = _resourcesDataUI.FirstOrDefault(x =>
                x.ResourceAsData.ResourceConfig.TypeResource == data.ResourceConfig.TypeResource);

            if (resourceDataUI != null)
            {
                resourceDataUI.UpdateResource();
                return;
            }

#if UNITY_EDITOR
            Debug.LogError($"None resource data UI. Resource type: {data.ResourceConfig.TypeResource}");
#endif
        }

        private void OnChangeResources(List<ResourceData> dataList) =>
            GenerateResourcesPanel(dataList);

        private void OnRemoveResource(ResourceData data)
        {
            var resourceDataUI = _resourcesDataUI.FirstOrDefault(x =>
                x.ResourceAsData.ResourceConfig.TypeResource == data.ResourceConfig.TypeResource);

            if (resourceDataUI != null)
            {
                resourceDataUI.UpdateResource();
                return;
            }

#if UNITY_EDITOR
            Debug.LogError("None resource data UI");
#endif
        }

        private void GenerateResourcesPanel(List<ResourceData> dataList)
        {
            ClearResourcesPanel();

            dataList.Sort((x1, x2) =>
            {
                if (x1.ResourceConfig.SortPriority < x2.ResourceConfig.SortPriority)
                {
                    return -1;
                }

                return 1;
            });

            foreach (var data in dataList)
            {
                var resourceDataUI = Instantiate(_resourcePrefab, _resourcesParent);
                resourceDataUI.ChangeResourceData(data);
                _resourcesDataUI.Add(resourceDataUI);
            }
        }

        private void ClearResourcesPanel()
        {
            if (_resourcesParent == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Resource panels is null");
#endif
                return;
            }

            foreach (Transform child in _resourcesParent)
            {
                Destroy(child.gameObject);
            }

            _resourcesDataUI.Clear();
        }
    }
}