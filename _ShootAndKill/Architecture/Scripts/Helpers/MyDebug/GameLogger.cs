using NaughtyAttributes;
using R3;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Helpers.Debugging
{
    [AddComponentMenu("Debug/MyDebugger")]
    public class GameLogger : MonoBehaviour
    {
        [SerializeField] private string _prefix;
        [SerializeField] private Color _prefixColor;
        [SerializeField] private Color _debugColor;
        [SerializeField] private bool _onGame = true;
        [SerializeField] private bool _onConsole = true;
        [SerializeField, ReadOnly] private GameDebug _debug;

        [Inject]
        private void Initialize(GameDebug debug) {
            _debug = debug;
        }
        
        public void Log(string text, Object sender = null, Color textColor = default) {
            textColor = textColor == default ? _debugColor : textColor;
            
            if (_onGame) {
                var newText = ConvertToGameCongoleText(text, textColor);
                _debug.Log(newText);
            }

            if (_onConsole)
                Debug.Log(
                    ConvertToUnityConsoleText(text, textColor),
                    sender);
        }

        public void WarningLog(string text, Object sender = null, Color textColor = default) {
            if (_onGame) {
                var ntext = ConvertToGameCongoleText(text, textColor);
                _debug.WarningLog(ntext);
            }
            
            if (_onConsole) {
                if (textColor == default) textColor = Color.yellow;
                Debug.LogWarning(
                    ConvertToUnityConsoleText(text, textColor),
                    sender);
            }
        }

        public void ErrorLog(string text, Object sender = null,Color textColor = default) {
            textColor = textColor == default ? _debugColor : textColor;
            
            if (_onGame) {
                var ntext = ConvertToGameCongoleText(text, textColor);
                _debug.ErrorLog(ntext);
            }

            if (_onConsole)
                Debug.LogError(
                    ConvertToUnityConsoleText(text, textColor),
                    sender);
        }

        private string ConvertToUnityConsoleText(string text, Color textColor) {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_prefixColor)}>{_prefix}</color>" +
                   $"\n<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{text}</color>";
        }

        private string ConvertToGameCongoleText(string text, Color textColor) {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_prefixColor)}>{_prefix}</color>: " +
                   $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{text}</color>";
        }
    }
}