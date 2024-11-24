using System;
using CraftingSystem.Core;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace CraftingSystem.UI
{
    public class CraftingItemUI : MonoBehaviour, IInitialize
    {
        [SerializeField] private Image _imageRecipe;
        [SerializeField] private Button _buttonSelect;

        private CraftingRecipe _currentRecipe;
        
        public bool IsEnable { get; set; }

        public Action<CraftingRecipe> OnSelectRecipe { get; set; }
        
        public void Initialize(params object[] objects)
        {
            _currentRecipe = objects[0] as CraftingRecipe;
            
            if (_imageRecipe != null)
            {
                _imageRecipe.sprite = _currentRecipe.OutputResource.ResourceConfig.ResourceSprite;
            }
            
            if (_buttonSelect != null)
            {
                _buttonSelect.onClick.AddListener(() => OnSelectRecipe?.Invoke(_currentRecipe));
            }
        }
    }
}