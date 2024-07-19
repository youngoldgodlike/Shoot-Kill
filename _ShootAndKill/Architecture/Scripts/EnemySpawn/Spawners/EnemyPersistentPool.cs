using System;
using Architecture.GameData;
using Architecture.GameData.Configs;
using Architecture.MinePool;
using Enemies.EnemiesVariety;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class EnemyPersistentPool : PersistentPool<Enemy>
    {
        private int _counter = 0;
        private readonly EnemyConfig _config;
        private float _statsMultiplier;
        private float _expMultiplier = 1f;
        
        #region Stats

        private float health => _config.health * _statsMultiplier;

        private float damage => _statsMultiplier > 1
            ? _config.damage + (((_config.damage * _statsMultiplier) - _config.damage) / 3)
            : _config.damage * _statsMultiplier;
        private float motionSpeed => _config.motionSpeed;

        private float expAmount => _config.expQuantity * Mathf.Clamp(_statsMultiplier, 1f, Mathf.Infinity) * _expMultiplier;
        
        #endregion

        public EnemyPersistentPool(Func<Enemy> create, Action<Enemy> @return, Action<Enemy> get,EnemyConfig config, float statsMultp ,int capacity) : base(create, @return, get, capacity) {
            _config = config;
            _statsMultiplier = statsMultp;
            OnCreateItem += x => {
                x.parent.name = $"{_counter}. {x.parent.name}";
                _counter++;
            };
            OnGetItem += InitEnemy;
        }

        public void Reinitialize(EnemyPreset preset) {
            _statsMultiplier = preset.statsMultiplier;
            Debug.Log($"STATSI: {_statsMultiplier}");
            _expMultiplier = preset.expMultiplier;
            
            for (var i = count; i < preset.requiredQuantity; i++) {
                Create();
            }
        }

        private void InitEnemy(Enemy enemy) {
            enemy.Initialize(expAmount);
            enemy.health.Initialize(health);
            enemy.dmgInfliction.Initialize(damage);
            enemy.motion.Initialize(motionSpeed);
        }
    }
}