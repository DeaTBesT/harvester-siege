using System;
using CraftingSystem.Core;
using UI;
using UnityEngine;

namespace CraftingSystem.UI
{
    public class CraftingUIPanel : UIPanel
    {
        [SerializeField] private CraftingRecipe[] _craftingRecipes;

        [Header("Crafting recipes canvas")]
        [SerializeField] private Transform _buttonsParent;
        [SerializeField] private CraftingItemUI _craftingItemUI;

        public Action<CraftingRecipe> OnSelectRecipe; 
        
        private void Start() => 
            GenerateButtons();

        private void ClearButtons()
        {
            foreach (Transform button in _buttonsParent)
            {
                Destroy(button.gameObject);
            }
        }
        
        private void GenerateButtons()
        {
            ClearButtons();
            
            foreach (var recipe in _craftingRecipes)
            {
                var newItem = Instantiate(_craftingItemUI.gameObject, _buttonsParent);

                if (newItem.TryGetComponent(out CraftingItemUI craftingItem))
                {
                    craftingItem.Initialize(recipe);
                    craftingItem.OnSelectRecipe += SelectRecipe;
                }
            }
        }

        private void SelectRecipe(CraftingRecipe recipe) => 
            OnSelectRecipe?.Invoke(recipe);
    }
}