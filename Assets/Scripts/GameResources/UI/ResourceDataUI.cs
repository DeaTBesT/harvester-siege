using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameResources.UI
{
    public class ResourceDataUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private TextMeshProUGUI _textAmmount;
        [SerializeField] private Image _resourceImage;

        public ResourceData ResourceDataConfig { get; private set; }

        public void ChangeResourceData(ResourceData resourceData)
        {
            ResourceDataConfig = resourceData;
            _resourceImage.sprite = ResourceDataConfig.gameResourceDataConfig.ResourceSprite;
            UpdateResource();
        }

        public void UpdateResource() => 
            _textAmmount.text = $"{ResourceDataConfig.AmountResource}";
    }
}