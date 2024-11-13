using UnityEngine;

namespace UI
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private Canvas _panelCanvas;
        
        public void Show() => 
            _panelCanvas.enabled = true;

        public void Hide() => 
            _panelCanvas.enabled = false;
    }
}