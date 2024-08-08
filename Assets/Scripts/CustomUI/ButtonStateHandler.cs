using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomUI
{
    public class ButtonStateHandler
    {
        private TextMeshPro _textMeshPro;
        private Color _textOriginalColor;
        private Color _iconOriginalColor;
        private Renderer _iconRenderer;
        private List<MonoBehaviour> _buttonBehaviours;
        private Transform _buttonHighLightComponent;
        private bool _isInitialized = false;
        
        public ButtonStateHandler() { }

        public void Initialize(GameObject button)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            
            var iconParent = button.transform.Find("IconAndText");
            _textMeshPro = iconParent.GetComponentInChildren<TextMeshPro>();
            _iconRenderer = iconParent.Find("UIButtonSquareIcon").
                gameObject.GetComponent<Renderer>();
            _buttonHighLightComponent =
                button.transform.Find("CompressableButtonVisuals");
            _buttonBehaviours = button.GetComponents<MonoBehaviour>().ToList();
            _textOriginalColor = _textMeshPro.color;
            _iconOriginalColor = _iconRenderer.material.color;
        }

        public void SetState(bool active)
        {
            foreach (var b in _buttonBehaviours)
            {
                b.enabled = active;
            }
            _buttonHighLightComponent.gameObject.SetActive(active);
            _textMeshPro.color = active ? _textOriginalColor : Color.gray;
            _iconRenderer.material.color = active ? _iconOriginalColor : Color.gray;
        } 
    }
}