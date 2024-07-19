using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class SpawnProcesses : MonoBehaviour
    {
        [SerializeField] private SpawnProcessLabel _prefabLabel;

        private RectTransform _rectTransform;

        private readonly Dictionary<SerializableGuid, SpawnProcessLabel> _spawnProcesses = new();

        [ShowNativeProperty] private float height => _rectTransform.rect.height;
        private int labelsCount => _spawnProcesses.Count;

        public event Action<SerializableGuid> OnProcessCancellation = delegate { };

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void AddLabel(SpawnRequest request) {
            var label = Instantiate(_prefabLabel, transform);
            var labelRect = label.GetComponent<RectTransform>();
            
            _spawnProcesses.Add(request.guid, label);
            labelRect.localPosition = new Vector3(0f, (height - (labelRect.rect.height * 2 + 20) * labelsCount) / 2, 0f);
            
            label.Set(request.identification, request.guid);
            label.OnCancelSpawn += Guid => {
                Destroy(_spawnProcesses[Guid].gameObject);
                _spawnProcesses.Remove(Guid);
                OnProcessCancellation.Invoke(Guid);
            };
            
        }

        public void RemoveLabel(SerializableGuid requestGuid) {
            Destroy(_spawnProcesses[requestGuid].gameObject);
        }
    }
}