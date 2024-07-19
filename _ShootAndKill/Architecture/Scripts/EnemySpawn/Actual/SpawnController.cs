using System;
using Architecture.GameData;
using Cysharp.Threading.Tasks;
using Enemies.EnemiesVariety;
using Helpers.Debugging;
using R3;
using SpawnSystem.TestSpawner;
using UnityEngine;
using static Cysharp.Threading.Tasks.UniTask;

namespace SpawnSystem
{
    public class SpawnController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private WaveCreator _spawner;
        [SerializeField] private SpawnPositionFinder _finder;
        [SerializeField] private Transform _enemiesParent;
        [SerializeField] private Transform _player;
        [SerializeField] private GameLogger _logger;
        [SerializeField] private BossCreator _bossCreator;

        [Header("Parameters")] 
        [SerializeField] private float _waveDuration;

        private readonly SpawnerData _data = new();
        private RegularSpawner _regular;
        private OnetimeSpawner _onetime;

        public event Action<WaveData> OnWaveEnd = delegate { };
        public readonly ReactiveCommand<string> EnemyKilled = new();

        private void Initialize() {
            _regular = new RegularSpawner(_enemiesParent, _player, _finder, this);
            _onetime = new OnetimeSpawner(_enemiesParent, _player, _finder, this);
            
            _regular.OnEnemyDied += x => EnemyKilled.Execute(x.fullname);
            _onetime.OnEnemyDied += x => EnemyKilled.Execute(x.fullname);

            _bossCreator.OnRequirementsComplete += CreateBoss;
        }

        private void CreateBoss() {
            _onetime.Spawn(_spawner.CreateBoss());
        }

        public void Start() {
            Initialize();
            GameProcess().Forget();
        }

        private async UniTask GameProcess() {
            while (true) {
                _logger.Log("Creating spawn process", this);
                CreateSpawnProcess();
            
                _logger.WarningLog("Waiting for next wave...", this, Color.yellow);
                await WaitForSeconds(_waveDuration, cancellationToken: destroyCancellationToken);
            }
        }

        private void CreateSpawnProcess() {
            if (_data.TryGetLastWave(out var data)) OnWaveEnd.Invoke(data);
            
            var request = _spawner.CreateSpawnRequest(_waveDuration);
            _data.AddWave(request);
            _logger.Log($"", this);
            _regular.Spawn(request.regularEnemies);
            _onetime.Spawn(request.onetimeEnemies);
        }
    }
}