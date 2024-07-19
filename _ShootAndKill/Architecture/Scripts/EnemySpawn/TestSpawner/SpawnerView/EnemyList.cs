using System.Collections.Generic;
using System.Linq;
using Architecture.GameData;
using NaughtyAttributes;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class EnemyList : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<EnemyPresenter> _enemies;
        [SerializeField, ReadOnly] private List<EnemyPresenter> _selectedEnemies;

        [SerializeField] private Transform _presetsParent;
        
        private void OnValidate() {
            _enemies.Clear();
            var comps = _presetsParent.GetComponentsInChildren<EnemyPresenter>();
            _enemies = comps.ToList();
        }

        private void Awake() {
            foreach (var enemyPresent in _enemies) {
                enemyPresent.OnEnemySelect += isSelected => {
                    if (isSelected) _selectedEnemies.Add(enemyPresent);
                    else _selectedEnemies.Remove(enemyPresent);
                };
            }
        }

        public IEnumerable<EnemyPreset> GetEnemyPresets() {
            if (_enemies.Count == 0) return null;
            
            return _selectedEnemies.Select(enemyPresent => enemyPresent.GetPreset()).ToList();
        }
    }
}