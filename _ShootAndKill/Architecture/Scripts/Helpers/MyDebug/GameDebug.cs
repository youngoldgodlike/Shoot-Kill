using System;
using _Shoot_Kill.Architecture.Scripts;
using R3;
using UnityEngine;
using Zenject;
using Image = UnityEngine.UI.Image;

namespace Helpers.Debugging
{
    public class GameDebug : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Image _background;
        [SerializeField] private GameConsole _gameConsole;

        [Header("Params")] 
        [SerializeField] private KeyCode _activateBtn;

        [SerializeField] private bool _isEnable;

        private IDisposable _disposable;

        [Inject]
        private void Initialize() {
            _background.gameObject.SetActive(false);

            if (_isEnable) Observable.EveryUpdate()
                    .Where(_ => Input.GetKeyDown(_activateBtn))
                    .Subscribe(_ => _background.gameObject.SetActive(!_background.gameObject.activeInHierarchy));

            //_disposable = Disposable.Combine(open);

            DontDestroyOnLoad(gameObject);
        }

        // private void OnDestroy() {
        //     _disposable.Dispose();
        // }

        /// <param name="text"> Important message</param>
        /// <param name="textColor"> Color</param>
        public void Log(string text) {
            _gameConsole.Insert(LogType.Log, text);
        }

        public void WarningLog(string text) {
            _gameConsole.Insert(LogType.Warning, text);
        }

        public void ErrorLog(string text) {
            _gameConsole.Insert(LogType.Error, text);
        }
    }
}
