using System.Collections.Generic;
using GameResources.Core;
using UnityEngine;

namespace CraftingSystem.Core
{
    [CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [SerializeField] private List<ResourceData> _inputResources; // Список ингредиентов и их количество
        [SerializeField] private ResourceData _outputResource; // Предмет, который будет получен

        public ResourceData OutputResource => _outputResource;
        public List<ResourceData> InputResources => _inputResources;
    }
}