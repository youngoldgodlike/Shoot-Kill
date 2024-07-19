using System;
using System.Collections.Generic;
using System.Threading;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.EnemiesVariety;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;
using Architecture.GameData;
using Architecture.MinePool;
using Cysharp.Threading.Tasks;
using Enemies.EnemiesVariety;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpawnSystem.TestSpawner
{
    public class OnetimeSpawner : EnemySpawner
    {
        public event Action<SpawnRequest> OnRequestCompletion = delegate { };
        
        public OnetimeSpawner(Transform spawnContainer, Transform player, SpawnPositionFinder finder, SpawnController controller) 
            : base(spawnContainer, player, finder, controller) {
        }

        public override void Spawn(SpawnRequest request) {
            var prnt = new GameObject(request.identification);
            prnt.transform.SetParent(spawnPlace);

            var requestTracker = new RequestTracker(ref request);
            requestTracker.OnRequestCompleted += () => OnRequestCompletion.Invoke(request);
            requestTracker.OnRequestCompleted += () => StopProcess(request);
            
            foreach (var preset in request.enemyPresets) {
                var pool = CreatePool(preset, prnt.transform);
                
                requestTracker.AddToTracking(pool as DisposablePool<Enemy>, preset);
                SpawnProcess(preset.spawnCooldown, request.cts, pool).Forget();
            }
        }


        protected override async UniTask SpawnProcess(float spawnCooldown, CancellationTokenSource cts, Pool<Enemy> pool) {
            do {
                await UniTask.WaitForSeconds(spawnCooldown, cancellationToken: controller.destroyCancellationToken);
                pool.Get();

                await UniTask.Yield(controller.destroyCancellationToken);
            } while (!pool.isEmpty);

            Debug.Log("Onetime Spawn Process ended");
        }

        protected override Enemy CreateEnemy(EnemyPreset data) {
            var enemy = builder
                .Create(data.enemyConfig)
                .WithParent(enemiesParent[data])
                .WithStatsMultiplier(data.statsMultiplier)
                .WithExpMultiplier(data.expMultiplier)
                .WithTarget(player)
                .Build();

            enemy.GetComponent<Health>().onDieProcessEnd.AddListener(() => {
                EnemyDied(data.enemyConfig);
                Object.Destroy(enemy.parent.gameObject);
            });

            return enemy;
        }

        protected override void GetFromPool(Enemy enemy) {
            enemy.onSpawn.Invoke();
            enemy.parent.gameObject.SetActive(true);
            enemy.agent.Warp(positionFinder.GetSpawnPos());
            // enemy.parent.transform.position = positionFinder.GetSpawnPos();
        }

        protected override void ReturnToPool(Enemy enemy) {
            enemy.parent.gameObject.SetActive(false);
        }

        protected override Pool<Enemy> CreatePersonalPool(EnemyPreset preset) {
            var pool = new DisposablePool<Enemy>(()=>CreateEnemy(preset), ReturnToPool, GetFromPool,
                preset.requiredQuantity);

            return pool;
        }

        public class RequestTracker
        {
            private readonly List<SerializableGuid> _presetsGuids = new();

            public event Action OnRequestCompleted = delegate { };
            
            public RequestTracker(ref SpawnRequest request) {
                foreach (var preset in request.enemyPresets) {
                    _presetsGuids.Add(preset.guid);
                }
            }

            public bool AddToTracking(DisposablePool<Enemy> trackablePool,EnemyPreset trackablePreset) {
                if (trackablePool == null) return false;

                trackablePool.OnDispose += () => PresetComplete(trackablePreset);
                
                return true;
            }

            public void PresetComplete(EnemyPreset preset) {
                _presetsGuids.Remove(preset.guid);
                Check();
            }

            private void Check() {
                if(_presetsGuids.Count == 0) OnRequestCompleted.Invoke();
            }
        }
    }
}