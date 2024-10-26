using GameResources.Core;
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

        public ResourceData ResourceAsData { get; private set; }

        public void ChangeResourceData(ResourceData resourceData)
        {
            ResourceAsData = resourceData;
            _resourceImage.sprite = ResourceAsData.ResourceConfig.ResourceSprite;
            UpdateResource();
        }

        public void UpdateResource() => 
            _textAmmount.text = $"{ResourceAsData.AmountResource}";
    }
}