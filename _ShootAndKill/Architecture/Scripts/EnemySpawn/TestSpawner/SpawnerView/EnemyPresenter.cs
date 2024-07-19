using System;
using Architecture.GameData;
using Architecture.GameData.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpawnSystem.TestSpawner
{
    public class EnemyPresenter : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;

        [Header("Data")]
        [SerializeField] private TMP_InputField _spawnQuantity;
        [SerializeField] private TMP_InputField _spawnCooldown;
        [SerializeField] private EnemyConfig _config;

        private bool _isSelected;
        private int _enemySpawnCount = 1;
        private int _enemySpawnCooldown = 1;
        
        private Color baseColour => Color.white;
        private Color selectedColor => Color.cyan;

        public event Action<bool> OnEnemySelect = delegate { };
        
        
        private void Awake() {
            if (_spawnQuantity.text == string.Empty) _spawnQuantity.text = "1";
            if (_spawnCooldown.text == string.Empty) _spawnCooldown.text = "1";
            _spawnQuantity.onValueChanged.AddListener(str => {
                if (str == string.Empty) return;

                try {
                    _enemySpawnCount = int.Parse(str);
                }
                catch (Exception e) {
                    _spawnQuantity.text = string.Empty;
                    Console.WriteLine(e);
                    throw;
                }
            });
            _spawnCooldown.onValueChanged.AddListener(str => {
                if (str == string.Empty) return;

                try {
                    _enemySpawnCooldown = int.Parse(str);
                }
                catch (Exception e) {
                    _spawnCooldown.text = string.Empty;
                    Console.WriteLine(e);
                    throw;
                }
            });
            _button.onClick.AddListener(Select);
        }

        private void OnValidate() {
            _name.text = _config.fullname;
        }

        private void Select() {
            _isSelected = !_isSelected;
            _background.color = _isSelected ? selectedColor : baseColour;

            OnEnemySelect.Invoke(_isSelected);
        }

        public EnemyPreset GetPreset() {
            return new EnemyPreset(_config, _enemySpawnCount, _enemySpawnCooldown);
        }
    }
}