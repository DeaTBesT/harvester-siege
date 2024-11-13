using System;
using Core;
using CraftingSystem.Core;
using CraftingSystem.UI;
using Enums;
using Interfaces;
using Mirror;
using UI;
using UnityEngine;
using Utils;

namespace CraftingSystem
{
    public class CraftingPlace : NetworkBehaviour, IInteractable, IInitialize, IDeinitialize
    {
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

        public void Initialize(params object[] objects)
        {
            _craftingUIPanel = objects[0] as CraftingUIPanel;
            _timerUIPanel = objects[1] as TimerUIPanel;
            _timer =  objects[2] as Timer;

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

            _currentRecipe = craftingRecipe;
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
            _craftingState = CraftingState.SelectRecipe;

        private void OnTimerForceStop() => 
            _timerUIPanel.Hide();

        private void OnSelectRecipe(CraftingRecipe recipe)
        {
            StartCraft(recipe);
            CloseCraftingPanel();
            _onFinishInteract?.Invoke();
        }
    }
}