using UnityEngine;

namespace GameResources
{
    [CreateAssetMenu(menuName = "Game resources/New resource")]
    public class GameResourceData : ScriptableObject
    {
        [SerializeField] private string _title = "empty";
        [SerializeField] private string _description = "empty";

        [SerializeField] private Sprite _resourceSprite;

        [SerializeField] private GameResourceType _gameResourceType;

        [Space]
        [SerializeField] private GameObject _resourcePrefab;

        public string Title => _title;
        public string Description => _description;
        public Sprite ResourceSprite => _resourceSprite;
        public GameResourceType TypeGameResource => _gameResourceType;
        public GameObject ResourcePrefab => _resourcePrefab;
    }
}