using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

namespace Helpers.Debugging
{
    public class ConsoleDebug : MonoBehaviour
    {
        [SerializeField] private string _prefix;
        [SerializeField] private Color _prefixColor;
        [SerializeField] private Color _debugColor;
        [SerializeField] private bool _enableLogging = true;

        public void Log(string text,Object obj = null,Color textColor = default) {
            if(!_enableLogging) return;
            Debug.Log(
                $"<color=#{ColorUtility.ToHtmlStringRGB(_prefixColor)}>{_prefix}</color>" +
                $"\n<color=#{ColorUtility.ToHtmlStringRGB(textColor == default ? _debugColor : textColor)}>{text}</color>",
                obj);
        }
    }
}