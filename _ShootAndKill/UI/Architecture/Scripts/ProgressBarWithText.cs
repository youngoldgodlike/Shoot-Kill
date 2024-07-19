using TMPro;
using UnityEngine;

namespace Assets.UI.Architecture.Scripts
{
    public class ProgressBarWithText : ProgressBarWidget
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetProgressWithText(float currentValue, float maxValue, string message)
        {
            _text.text = message;
            SetProgress(currentValue,maxValue);
        }

        public void SetText(string message) => _text.text = message;
    }
}