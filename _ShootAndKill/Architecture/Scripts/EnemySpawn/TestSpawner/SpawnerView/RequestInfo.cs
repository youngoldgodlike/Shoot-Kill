using System.Collections.Generic;
using Architecture.GameData;
using TMPro;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class RequestInfo : MonoBehaviour
    {
        [SerializeField] private EnemyList _enemyList;
        [SerializeField] private TMP_InputField _processName;
        [SerializeField] private TMP_Dropdown _spawnType;

        private void OnValidate() {
            _spawnType.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData> {
                new(SpawnType.Regular.ToString()),
                new(SpawnType.Onetime.ToString())
            };
            _spawnType.options = options;
        }

        public IEnumerable<EnemyPreset> GetEnemyPresets() {
            return _enemyList.GetEnemyPresets();
        }

        public string GetProcessName() {
            return _processName.text != string.Empty ? _processName.text : "No name";
        }

        public SpawnType GetSpawnType() {
            switch (_spawnType.value) {
                case 0:
                    return SpawnType.Regular;
                case 1:
                    return SpawnType.Onetime;
                default:
                    Debug.LogWarning("Not enough spawn types in script. Type ID: " + _spawnType);
                    return default;
            }
        }
    }
}