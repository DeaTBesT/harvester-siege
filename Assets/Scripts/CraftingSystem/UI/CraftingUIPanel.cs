using System;
using CraftingSystem.Core;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace CraftingSystem.UI
{
    public class CraftingUIPanel : UIPanel
    {
        [SerializeField] private CraftingRecipe[] _craftingRecipes;

        [Header("Crafting recipes canvas")]
        [SerializeField] private Transform _buttonsParent;
        [SerializeField] private Button _buttonPrefab;

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
                var newButton = Instantiate(_buttonPrefab.gameObject, _buttonsParent);

                if (newButton.TryGetComponent(out Image image))
                {
                    image.sprite = recipe.OutputResource.ResourceConfig.ResourceSprite;
                }
                
                if (newButton.TryGetComponent(out Button button))
                {
                    button.onClick.AddListener(() =>
                    {
                        SelectRecipe(recipe);
                    });
                }
            }
        }

        private void SelectRecipe(CraftingRecipe recipe) => 
            OnSelectRecipe?.Invoke(recipe);
    }
}