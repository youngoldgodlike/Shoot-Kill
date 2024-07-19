using System.Collections.Generic;
using System.Threading;
using Architecture.GameData;
using Architecture.GameData.Configs;
using Architecture.MinePool;
using Cysharp.Threading.Tasks;
using Enemies.EnemiesVariety;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpawnSystem.TestSpawner
{
    public class RegularSpawner : EnemySpawner
    {
        private readonly Dictionary<SerializableGuid, EnemyPersistentPool> _enemyPools = new();
        private readonly Dictionary<SerializableGuid, float> _enemyCooldown = new();
        private Dictionary<SerializableGuid, Transform> _enemyParent = new();

        public RegularSpawner(Transform spawnContainer, Transform player, SpawnPositionFinder finder, SpawnController controller) 
            : base(spawnContainer, player, finder, controller) {
        }

        // хуйня. убрать
        protected override Pool<Enemy> CreatePersonalPool(EnemyPreset preset) {
            var pool = new EnemyPersistentPool(() => CreateEnemy(preset), ReturnToPool, GetFromPool, preset.enemyConfig,
                preset.statsMultiplier, preset.requiredQuantity);
            
            return pool;
        }

        public override void Spawn(SpawnRequest request) {
            foreach (var preset in request.enemyPresets) {
                var guid = preset.enemyConfig.guid;
                _enemyCooldown[guid] = preset.spawnCooldown;

                if (_enemyPools.ContainsKey(guid)) {
                    _enemyPools[guid].Reinitialize(preset);
                }
                else {
                    var parent = new GameObject($"{preset.enemyConfig.fullname}").transform;
                    parent.SetParent(spawnPlace);
                    _enemyParent.Add(guid, parent);
                    _enemyPools.Add(guid,
                        new EnemyPersistentPool(() => CreateEnemy(preset.enemyConfig), ReturnToPool, GetFromPool,
                            preset.enemyConfig, preset.statsMultiplier, preset.requiredQuantity));
                    SpawnProcess(guid).Forget(); 
                }
            }
        }

        protected override async UniTask SpawnProcess(float spawnCooldown,
            CancellationTokenSource cts, Pool<Enemy> pool) {
            
            while (true) {
                await UniTask.WaitForSeconds(spawnCooldown, cancellationToken: cts.Token);
                await UniTask.WaitUntil(() => pool.Get(), cancellationToken: cts.Token);

                await UniTask.Yield(cts.Token);
            }
        }
        
        protected async UniTask SpawnProcess(SerializableGuid guid) {
            var pool = _enemyPools[guid];
            
            while (true) {
                await UniTask.WaitForSeconds(_enemyCooldown[guid], 
                    cancellationToken: controller.destroyCancellationToken);
                await UniTask.WaitUntil(() => pool.Get(), 
                    cancellationToken: controller.destroyCancellationToken);

                await UniTask.Yield();
            }
        }

        private Enemy CreateEnemy(EnemyConfig data) {
            var enemy = builder
                .Create(data)
                .WithTarget(player)
                .WithParent(_enemyParent[data.guid])
                .Build();
            
            enemy.health.onDieProcessEnd.AddListener(() => {
                EnemyDied(data);
                _enemyPools[data.guid].Return(enemy);
            });

            return enemy;
        }
        
        protected override Enemy CreateEnemy(EnemyPreset data) {
            // var enemy = builder
            //     .Create(data.enemyConfig)
            //     .WithTarget(player)
            //     .WithStatsMultiplier(data.statsMultiplier)
            //     .WithParent(enemiesParent[data])
            //     .Build();
            //
            // enemy.GetComponent<Health>().onDieProcessEnd.AddListener(() => {
            //     EnemyDied(data.enemyConfig);
            //     enemiesPools[data].Return(enemy);
            // });
            var enemy = Object.Instantiate(data.enemyConfig.enemyPrefab);
            
            return enemy.GetComponent<Enemy>();
        } // хуйня. убрать

        protected override void GetFromPool(Enemy enemy) {
            enemy.onSpawn.Invoke();
            enemy.parent.gameObject.SetActive(true);

            enemy.agent.Warp(positionFinder.GetSpawnPos());
            // var pos = positionFinder.GetSpawnPos();
            // enemy.parent.transform.position = pos;
        }

        protected override void ReturnToPool(Enemy enemy) {
            enemy.parent.gameObject.SetActive(false);
        }

        private void Destroy(Enemy enemy) {
            Object.Destroy(enemy.parent.gameObject);
        }
    }
}