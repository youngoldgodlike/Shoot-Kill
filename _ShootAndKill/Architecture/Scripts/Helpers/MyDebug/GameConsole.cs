using System;
using System.Collections.Generic;
using NaughtyAttributes;
using R3;
using UnityEngine;
using YG;

namespace Helpers.Debugging
{
    public class GameConsole : MonoBehaviour
    {
        [Header("Dependencies")] 
        [SerializeField] private RectTransform _container;

        [SerializeField] private ConsoleLog _prefab;
        [SerializeField] private Sprite _warning, _error;

        [Header("Params")] [SerializeField] private int _logCapacity;
        [SerializeField] private int _containerCapacity = 40;
        [SerializeField] private bool _showLog;

        [ShowNativeProperty] private float heightForLog => Mathf.Clamp(_container.rect.height / _containerCapacity, 10f, 20f);
        [ShowNativeProperty] private float containerHeight => _container.rect.height;
        [ShowNativeProperty] private int currentLogCapacity => _logs.Count;


        [SerializeField, ShowIf(nameof(_showLog))] 
        private List<ConsoleLog> _consoleLogsList;
        [SerializeField, ReadOnly, ResizableTextArea,ShowIf(nameof(_showLog))] 
        private string _viewLog;
        
        private readonly ObservableList<ConsoleLog> _logs = new();
        private IDisposable _disposable;

        private void Awake() {
            var containerTracker = Observable.EveryValueChanged(_container, container => container.rect)
                .Subscribe(_ => RefreshView());

            _disposable = Disposable.Combine(containerTracker);

            _logs.AnyValueChanged += (x) => RefreshView();
        }

        private void OnDestroy() {
            _disposable.Dispose();
        }

        private void RefreshView() {
            var reversedLog = new List<ConsoleLog>(_logs);
            reversedLog.Reverse();
            
            for (var i = 0; i < reversedLog.Count; i++) {
                var yPos = -containerHeight / 2 + heightForLog / 2 + heightForLog * i;
                reversedLog[i].SetRect(_container.rect.width, heightForLog, yPos);
            }
        }

        private int _createdLogsCount = 0;
        public void Insert(LogType logType, string text) {
            var sprite = logType switch {
                LogType.Log => null,
                LogType.Warning => _warning,
                LogType.Error => _error,
                _ => null
            };

            var log = Instantiate(_prefab, _container);
            log.name += $"{++_createdLogsCount}";
            _viewLog += $"{log.name} has {sprite} image\n";
            log.Initialize(text, sprite);

            _logs.Add(log);
            _consoleLogsList.Add(log);
            
            if (_logs.Count <= _logCapacity) return;
            
            var logToDestroy = _logs[0];
            _logs.Remove(logToDestroy);
            _consoleLogsList.Remove(logToDestroy);
            Destroy(logToDestroy.gameObject);
        }
    }
}