using Core;
using CraftingSystem.UI;
using UI;
using UnityEngine;
using Utils.TimerSystem;
using Utils.TimerSystem.Core;

namespace CraftingSystem
{
    public class CraftingPlaceInitializer : EntityInitializer
    {
        private const float HOLDING_TIME = 2f;
        private const float DELAYED_TIME = 0.5f;
        
        [SerializeField] private CraftingUIPanel _craftingUIPanel;
        [SerializeField] private TimerUIPanel _timerUIPanel;
        [SerializeField] private CraftingPlace _craftingPlace;

        private Timer _timer;
        
        protected override void OnValidate()
        {
            if (_craftingUIPanel == null)
            {
                _craftingUIPanel = GetComponent<CraftingUIPanel>();
            }
            
            if (_timerUIPanel == null)
            {
                _timerUIPanel = GetComponent<TimerUIPanel>();
            }  
            
            if (_craftingPlace == null)
            {
                _craftingPlace = GetComponent<CraftingPlace>();
            }
        }

        public override void Initialize(params object[] objects)
        {
            _timer = new DelayedTimer(_craftingPlace, HOLDING_TIME, DELAYED_TIME);
            
            _timerUIPanel.Initialize(HOLDING_TIME);
            _craftingPlace.Initialize(_craftingUIPanel,
                _timerUIPanel,
                _timer);
        }

        public override void Deinitialize(params object[] objects) => 
            _craftingPlace.Deinitialize();
    }
}