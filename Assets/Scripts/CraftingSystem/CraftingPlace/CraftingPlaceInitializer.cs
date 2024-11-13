using Core;
using CraftingSystem.UI;
using UI;
using UnityEngine;
using Utils;

namespace CraftingSystem
{
    public class CraftingPlaceInitializer : EntityInitializer
    {
        private const float HOLDING_TIME = 5f;
        
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

        public override void Initialize()
        {
            _timer = new Timer(_craftingPlace, HOLDING_TIME);
            
            _timerUIPanel.Initialize(HOLDING_TIME);
            _craftingPlace.Initialize(_craftingUIPanel,
                _timerUIPanel,
                _timer);
        }

        public override void Deinitialize() => 
            _craftingPlace.Deinitialize();
    }
}