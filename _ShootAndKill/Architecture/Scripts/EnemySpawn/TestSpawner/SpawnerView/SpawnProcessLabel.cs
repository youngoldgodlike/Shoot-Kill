using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpawnSystem.TestSpawner
{
    public class SpawnProcessLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _enemyInfo;
        [SerializeField] private Button _cancelBtn;
        [SerializeField] private SerializableGuid _guid;

        public event Action<SerializableGuid> OnCancelSpawn = delegate {  };

        private void Awake() {
            _cancelBtn.onClick.AddListener(() => OnCancelSpawn.Invoke(_guid));
        }

        public void Set(string index, SerializableGuid guid) {
            _enemyInfo.text = $"{index}";
            _guid = guid;
        }
    }
}