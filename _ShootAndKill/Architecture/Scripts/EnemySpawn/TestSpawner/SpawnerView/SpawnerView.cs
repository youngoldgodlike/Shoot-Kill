using System;
using System.Linq;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SpawnSystem.TestSpawner
{
    public class SpawnerView : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _spawnBtn;
        [SerializeField] private SpawnProcesses _labelsSpawner;
        [SerializeField] private RequestInfo _requestInfo;
        [SerializeField] private Vector2Slider _spawnRange;

        private IDisposable _disposable;

        public ReactiveProperty<Vector2> Range { get; } = new();

        public event Action<SpawnRequest> OnSpawnRequest = delegate { };
        public event Action<SerializableGuid> OnSpawnCancellation = delegate { };

        private void Awake() {
            _panel.SetActive(false);
            
            _spawnBtn.onClick.AddListener(CreateRequest);
            OnSpawnRequest += _labelsSpawner.AddLabel;
            _labelsSpawner.OnProcessCancellation += OnSpawnCancellation;
        }

        private void OnEnable() {
            var uiOpener = Observable
                .EveryUpdate(UnityFrameProvider.Update)
                .Where(_ => Input.GetKeyDown(KeyCode.Tab))
                .Subscribe(_ => OpenCloseView());
            
            _spawnRange.value.Subscribe(range => Range.Value = range);
            
            _disposable = Disposable.Combine(uiOpener);
        }

        private void OnDisable() {
            if (_disposable.IsUnityNull()) return;
            _disposable.Dispose();
        }

        private void OpenCloseView() {
            _panel.SetActive(!_panel.activeInHierarchy);
            Time.timeScale = _panel.activeInHierarchy ? 0f : 1f;
        }
        
        private void CreateRequest() {
            var enemyPresets = _requestInfo.GetEnemyPresets().ToList();

            if(enemyPresets.Count == 0) return;

            var newRequest = new SpawnRequest(enemyPresets, _requestInfo.GetProcessName(), _requestInfo.GetSpawnType());

            OnSpawnRequest.Invoke(newRequest);
        }

        public void RemoveRequest(SerializableGuid requestGuid) {
            _labelsSpawner.RemoveLabel(requestGuid);
        }
    }
}