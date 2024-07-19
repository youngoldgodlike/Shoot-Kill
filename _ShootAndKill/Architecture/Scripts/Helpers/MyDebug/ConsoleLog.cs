using Extensions;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers.Debugging
{
    public class ConsoleLog : MonoBehaviour
    {
        [SerializeField, ReadOnly] private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;

        private void OnValidate() {
            if(_rectTransform.IsUnityNull()) _rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(string text,Sprite sprite) {
            _text.text = text;

            if (sprite.IsUnityNull()) _image.color = Color.clear;
            else _image.sprite = sprite;
        }

        public void SetRect(float width,float height, float yPos) {
            _rectTransform.SetSize(new Vector2(width, height));
            _rectTransform.SetPivotAndAnchors(new Vector2(0.5f, 0.5f));
            _rectTransform.localPosition = new Vector3(0f, yPos);
        }
    }
}