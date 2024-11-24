using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using CraftingSystem.Core;
using CraftingSystem.CustomSerialization;
using CraftingSystem.UI;
using Enums;
using GameResources.Core;
using Interfaces;
using Managers;
using Mirror;
using Player;
using UI;
using UnityEngine;
using Utils.Networking;
using Utils.TimerSystem;
using Utils.TimerSystem.Core;

namespace CraftingSystem
{
    public class CraftingPlace : NetworkBehaviour, IInteractable, IInitialize, IDeinitialize, IInstantiateResource, INetworkLoad
    {
        private List<ResourceData> _requiredResources = new();

        private CraftingState _craftingState;
        private CraftingRecipe _currentRecipe;

        private NetworkIdentity _currentInteractor;

        private CraftingUIPanel _craftingUIPanel;
        private TimerUIPanel _timerUIPanel;

        private Timer _timer;

        private Action _onFinishInteract;

        public InteractType TypeInteract
        {
            get
            {
                switch (_craftingState)
                {
                    case CraftingState.SelectRecipe:
                        return InteractType.Toggle;
                    case CraftingState.Crafting:
                        return InteractType.Holding;
                    default:
                        return InteractType.Toggle;
                }
            }
        }

        public NetworkIdentity NetIdentity => netIdentity;
        public Action OnInteract { get; set; }
        public Action OnFinishInteract { get; set; }
        public bool IsEnable { get; set; }

        public override void OnStartServer() => 
            NetworkLoadManager.Instance.AddLoader(this);

        public override void OnStopServer() => 
            NetworkLoadManager.Instance.RemoveLoader(this);
        
        [Server]
        public void LoadDataServer(NetworkConnectionToClient conn)
        {
            var craftingState = (int)_craftingState;
            var selectedRecipe = NetworkScriptableObjectSerializer.SerializeScriptableObject(_currentRecipe);
            var resourceNameList = new List<string>();
            var resourceAmountList = new List<int>();

            foreach (var resourceData in _requiredResources)
            {
                resourceNameList.Add(
                    NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig));
                resourceAmountList.Add(resourceData.AmountResource);
            }
            
            var data = new CraftingPlaceData(craftingState, selectedRecipe, resourceNameList, resourceAmountList);

            var writer = new NetworkWriter();
            writer.WriteCraftingPlaceData(data);
            var writerData = writer.ToArray();

            LoadDataRpc(conn, writerData);
        }

        [TargetRpc]
        public void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData)
        {
            var reader = new NetworkReader(writerData);
            var data = reader.ReadCraftingPlaceData();

            _craftingState =  (CraftingState)data.CraftingState;
            _currentRecipe = (CraftingRecipe)NetworkScriptableObjectSerializer.DeserializeScriptableObject(data.SelectedRecipe);
            
            for (var i = 0; i < data.RequiredResourceNameList.Count; i++)
            {
                var resourceName = data.RequiredResourceNameList[i];
                var resourceAmount = data.RequiredResourceAmountList[i];

                var resourceConfig =
                    (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
                _requiredResources.Add(new ResourceData(resourceConfig, resourceAmount));
            }
        }
        
        public void Initialize(params object[] objects)
        {
            _craftingUIPanel = objects[0] as CraftingUIPanel;
            _timerUIPanel = objects[1] as TimerUIPanel;
            _timer = objects[2] as DelayedTimer;

            if (_timer != null)
            {
                _timer.OnTimerStart += OnTimerStart;
                _timer.OnChangedTime += OnChangedTime;
                _timer.OnTimerStop += OnTimerStop;
                _timer.OnTimerForceStop += OnTimerForceStop;
            }

            if (_craftingUIPanel != null)
            {
                _craftingUIPanel.OnSelectRecipe += OnSelectRecipe;
            }

            _timerUIPanel.Hide();
            _craftingUIPanel.Hide();
        }

        public void Deinitialize()
        {
            if (_timer != null)
            {
                _timer.OnTimerStart -= OnTimerStart;
                _timer.OnChangedTime -= OnChangedTime;
                _timer.OnTimerStop -= OnTimerStop;
                _timer.OnTimerForceStop -= OnTimerForceStop;
            }

            if (_craftingUIPanel != null)
            {
                _craftingUIPanel.OnSelectRecipe -= OnSelectRecipe;
            }
        }

        public void StartCraft(CraftingRecipe craftingRecipe)
        {
            if (craftingRecipe == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Recipe is null");
#endif
                return;
            }

            var recipe = NetworkScriptableObjectSerializer.SerializeScriptableObject(craftingRecipe);
            StartCraftCmd(recipe);
        }

        [Command(requiresAuthority = false)]
        private void StartCraftCmd(string recipeName) =>
            StartCraftRpc(recipeName);

        [ClientRpc]
        private void StartCraftRpc(string recipeName)
        {
            var recipe = (CraftingRecipe)NetworkScriptableObjectSerializer.DeserializeScriptableObject(recipeName);

            _currentRecipe = recipe;
            _craftingState = CraftingState.Crafting;
        }

        public bool TryInteract(IInteractor interactor, Action onFinishInteract = null)
        {
            _onFinishInteract = onFinishInteract;

            switch (_craftingState)
            {
                case CraftingState.SelectRecipe:
                {
                    _currentInteractor = interactor.InteractableNetId;
                    OpenCraftingPanel();
                }
                    return true;
                case CraftingState.Crafting:
                {
                    _currentInteractor = interactor.InteractableNetId;
                    StartHolding(interactor);
                }
                    return true;
                default:
                    return false;
            }
        }

        public void StartHolding(IInteractor interactor) =>
            _timer.Start();

        public void StopHolding()
        {
            switch (_craftingState)
            {
                case CraftingState.Crafting:
                {
                    _timer.Stop();
                    InsertResources();
                }
                    break;
            }
        }

        public void FinishInteract(IInteractor interactor)
        {
            switch (_craftingState)
            {
                case CraftingState.SelectRecipe:
                {
                    CloseCraftingPanel();
                }
                    break;
            }
        }

        public void ForceFinishInteract(IInteractor interactor)
        {
            throw new NotImplementedException();
        }

        private void OpenCraftingPanel()
        {
            if (_currentInteractor == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Interactor is null");
#endif
                return;
            }

            if (_currentInteractor.TryGetComponent(out EntityController entityController))
            {
                entityController.DiactivateMoveEntity();
                _craftingUIPanel.Show();
            }
        }

        private void CloseCraftingPanel()
        {
            if (_currentInteractor.TryGetComponent(out EntityController entityController))
            {
                entityController.ActivateMoveEntity();
                _craftingUIPanel.Hide();
                _currentInteractor = null;
            }
        }

        private void OnTimerStart() =>
            _timerUIPanel.Show();

        private void OnChangedTime(float time) =>
            _timerUIPanel.SetTime(time);

        private void OnTimerStop() =>
            OnTimerStopCmd();

        [Command(requiresAuthority = false)]
        private void OnTimerStopCmd() =>
            OnTimerStopRpc();

        [ClientRpc]
        private void OnTimerStopRpc() =>
            _craftingState = CraftingState.SelectRecipe;

        private void OnTimerForceStop() =>
            _timerUIPanel.Hide();

        private void OnSelectRecipe(CraftingRecipe recipe)
        {
            OnSelectRecipeCmd(NetworkScriptableObjectSerializer.SerializeScriptableObject(recipe));
            _onFinishInteract?.Invoke();
        }

        [Command(requiresAuthority = false)]
        private void OnSelectRecipeCmd(string recipeName) =>
            OnSelectRecipeRpc(recipeName);

        [ClientRpc]
        private void OnSelectRecipeRpc(string recipeName)
        {
            var recipe = (CraftingRecipe)NetworkScriptableObjectSerializer.DeserializeScriptableObject(recipeName);

            StartCraft(recipe);
            _requiredResources.Clear();
            _requiredResources = recipe.InputResources
                .Select(resource => new ResourceData(resource.ResourceConfig, resource.AmountResource))
                .ToList();
        }

        private void InsertResources()
        {
            if (_currentInteractor == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Interactor is null");
#endif
                return;
            }

            if (!_currentInteractor.TryGetComponent(out PlayerInventoryController inventoryController))
            {
                return;
            }

            foreach (var removeResource in inventoryController.ResourcesData
                         .Select(resource => CheckRequiredResources(resource))
                         .Where(removeResource => removeResource != null))
            {
                RemoveRequiredResources(removeResource);
                inventoryController.RemoveResource(removeResource);
            }
        }

        /// <summary>
        /// Search resource in requested resources
        /// </summary>
        /// <param name="resourceData">Inserted resource</param>
        /// <returns>Removed resource data</returns>
        private ResourceData CheckRequiredResources(ResourceData resourceData)
        {
            var requiredResource = _requiredResources.FirstOrDefault(x =>
                x.ResourceConfig.TypeResource == resourceData.ResourceConfig.TypeResource);

            if (requiredResource == null)
            {
                return null;
            }

            var removedAmount = Mathf.Min(resourceData.AmountResource, requiredResource.AmountResource);

            return new ResourceData(resourceData.ResourceConfig, removedAmount);
        }

        private void RemoveRequiredResources(ResourceData resourceData)
        {
            var resourceName = NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig);
            var resourceAmount = resourceData.AmountResource;

            RemoveRequiredResourcesCmd(resourceName, resourceAmount);
        }

        [Command(requiresAuthority = false)]
        private void RemoveRequiredResourcesCmd(string resourceName, int amount) =>
            RemoveRequiredResourcesRpc(resourceName, amount);

        [ClientRpc]
        private void RemoveRequiredResourcesRpc(string resourceName, int amount)
        {
            var resourceConfig =
                (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
            var resourceData = _requiredResources.FirstOrDefault(x =>
                x.ResourceConfig.TypeResource == resourceConfig.TypeResource);

            if (resourceData == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Resource data is null");
#endif
                return;
            }

            resourceData.RemoveResource(amount);

            if (resourceData.AmountResource <= 0)
            {
                _requiredResources.Remove(resourceData);

                if (isServer)
                {
                    CheckFinishCraft();
                }
            }
        }

        private void CheckFinishCraft()
        {
            if (_requiredResources == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Required resources list is null");
#endif
                return;
            }

            if (_requiredResources.Count > 0)
            {
                return;
            }

            FinishCraft();
        }

        private void FinishCraft()
        {
            var resourceData = _currentRecipe.OutputResource;

            var resourceName = NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig);
            var resourceAmount = resourceData.AmountResource;

            InstantiateResource(resourceName, resourceAmount);
            FinishCraftCmd();
        }

        [Command(requiresAuthority = false)]
        private void FinishCraftCmd() =>
            FinishCraftRpc();

        [ClientRpc]
        private void FinishCraftRpc()
        {
            _craftingState = CraftingState.SelectRecipe;
            _currentRecipe = null;
        }

        public void InstantiateResource(string resourceName, int amount)
        {
            if (isServer)
            {
                InstantiateResourceServer(resourceName, amount);
            }
            else
            {
                InstantiateResourceCmd(resourceName, amount);
            }
        }

        [Command(requiresAuthority = false)]
        public void InstantiateResourceCmd(string resourceName, int amount) =>
            InstantiateResourceServer(resourceName, amount);

        [Server]
        public void InstantiateResourceServer(string resourceName, int amount) =>
            ResourceData.InstantiateResource(resourceName, amount, transform.position);
    }
}