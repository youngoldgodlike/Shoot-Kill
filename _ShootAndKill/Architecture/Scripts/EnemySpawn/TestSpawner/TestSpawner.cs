using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class TestSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnerView _spawnerUI;
        [SerializeField] private Transform _spawnPlace, _player;
        [SerializeField] private SpawnPositionFinder _finder;

        // [SerializeField] private PositionFinderType _finderType;
        //
        // [SerializeField, ShowIf(nameof(_finderType), PositionFinderType.FixedPositions)]
        // private List<SpawnPosition> _spawnPositions;
        //
        // [SerializeField,ShowIf(nameof(_finderType),PositionFinderType.AroundPlayer)] 
        // private Vector2 _spawnRange = new(30, 40);
        
        private SpawnerController _controller;

        private void Awake() {
            _controller = new SpawnerController.Builder()
                .Create(_spawnerUI, new SpawnerModel())
                .WithTarget(_player)
                .WithSpawnHolder(_spawnPlace)
                .WithSpawnPositions(_finder)
                .Build();
        }
        
        public enum PositionFinderType
        {
            AroundPlayer,FixedPositions   
        }
    }
}

