using System;
using Architecture.GameData.Configs;
using Enemies.EnemiesVariety;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Shoot_Kill.Architecture.Scripts.EnemySpawn
{
    public class EnemyBuilder
    {
        private EnemyConfig _config;
        private Transform _target, _parent;
        private float _statsMultiplier = 1f;
        private float _expMultiplier = 1f;

        #region Stats

        private float health => _config.health * _statsMultiplier;
        private float damage => _config.damage * _statsMultiplier;
        private float motionSpeed => _config.motionSpeed;

        private float expAmount =>
            _config.expQuantity * Mathf.Clamp(_statsMultiplier, 1f, Mathf.Infinity) * _expMultiplier;
        
        #endregion
        
        public EnemyBuilder Create(EnemyConfig config) {
            _config = config;
            return this;
        }

        public EnemyBuilder WithTarget(Transform target) {
            _target = target;
            return this;
        }

        public EnemyBuilder WithStatsMultiplier(float multiplier) {
            if (multiplier < 0) 
                throw new ArgumentOutOfRangeException(nameof(multiplier));
            
            _statsMultiplier = multiplier;
            return this;
        }

        public EnemyBuilder WithExpMultiplier(float multiplier) {
            if (multiplier < 0) 
                throw new ArgumentOutOfRangeException(nameof(multiplier));

            _expMultiplier = multiplier;
            return this;
        }

        public EnemyBuilder WithParent(Transform parent) {
            _parent = parent;
            return this;
        }

        public Enemy Build() {
            var enemyObj = Object.Instantiate(_config.enemyPrefab.gameObject, _parent);
            enemyObj.name = _config.fullname;
            var enemy = enemyObj.GetComponentInChildren<Enemy>();

            enemy.Initialize(expAmount);
            enemy.health.Initialize(health);
            enemy.dmgInfliction.Initialize(damage).SetTarget(_target);
            enemy.motion.Initialize(motionSpeed).SetTarget(_target);

            Clear();
            return enemy;
        }

        private void Clear() {
            _config = null;
            _target = null;
            _parent = null;
            _statsMultiplier = 1f;
        }
    }
}