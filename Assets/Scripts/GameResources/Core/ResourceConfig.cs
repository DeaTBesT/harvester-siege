using Enums;
using UnityEngine;

namespace GameResources.Core
{
    [CreateAssetMenu(menuName = "Game resources/New resource")]
    public class ResourceConfig : ScriptableObject
    {
        [SerializeField] private string _title = "empty title";
        [SerializeField] private string _description = "empty description";

        [SerializeField] private Sprite _resourceSprite;

        [SerializeField] private ResourceType _resourceType;

        [SerializeField] private GameObject _resourcePrefab;
        
        public string Title => _title;
        public string Description => _description;
        public Sprite ResourceSprite => _resourceSprite;
        public ResourceType TypeResource => _resourceType;
        public GameObject ResourcePrefab => _resourcePrefab;
    }
}