using System;
using System.Collections.Generic;
using System.Threading;
using _Shoot_Kill.Architecture.Scripts.EnemySpawn;
using Architecture.GameData;
using Architecture.GameData.Configs;
using Architecture.MinePool;
using Cysharp.Threading.Tasks;
using Enemies.EnemiesVariety;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpawnSystem.TestSpawner
{
    public abstract class EnemySpawner
    {
        protected readonly EnemyBuilder builder = new();
        protected readonly Dictionary<EnemyPreset, Pool<Enemy>> enemiesPools = new();
        protected readonly Dictionary<EnemyPreset, Transform> enemiesParent = new();

        protected readonly Transform spawnPlace, player;
        protected readonly SpawnPositionFinder positionFinder;
        protected readonly SpawnController controller;

        public event Action<EnemyConfig> OnEnemyDied = delegate { };

        protected EnemySpawner(Transform spawnContainer, Transform player,SpawnPositionFinder finder,SpawnController controller) {
            spawnPlace = spawnContainer;
            this.player = player;
            positionFinder = finder;
            this.controller = controller;
        }

        public void StopProcess(SpawnRequest request) {
            foreach (var preset in request.enemyPresets) {
                request.cts.Cancel();
                // if (enemiesPools[preset] is EnemyPersistentPool pool)
                //     pool.SetReturn(x => Object.Destroy(x.parent.gameObject));
                enemiesParent[preset] = null;
                enemiesPools[preset] = null;
            }
        }

        protected Pool<Enemy> CreatePool(EnemyPreset preset, Transform requestParent) {
            var go = new GameObject($"{preset.enemyConfig.fullname}");
            var poolParent = Object.Instantiate(go, requestParent);
            Object.Destroy(go);
            enemiesParent.Add(preset, poolParent.transform);

            var pool = CreatePersonalPool(preset);
            enemiesPools.Add(preset, pool);

            return pool;
        }

        protected void EnemyDied(EnemyConfig enemy) {
            OnEnemyDied.Invoke(enemy);
        }

        public abstract void Spawn(SpawnRequest request);

        protected abstract UniTask SpawnProcess(float spawnCooldown,
            CancellationTokenSource cancellationTokenSource, Pool<Enemy> pool);
        protected abstract Enemy CreateEnemy(EnemyPreset data);
        protected abstract void GetFromPool(Enemy enemy);
        protected abstract void ReturnToPool(Enemy enemy);
        protected abstract Pool<Enemy> CreatePersonalPool(EnemyPreset preset);
    }
}