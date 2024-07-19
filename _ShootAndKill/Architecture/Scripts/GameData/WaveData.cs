using System;
using SpawnSystem.TestSpawner;
using UnityEngine;

namespace Architecture.GameData
{
    [Serializable]
    public struct WaveData
    {
        [SerializeField] private SpawnRequest _regularEnemies;
        [SerializeField] private SpawnRequest _onetimeEnemies;
        [SerializeField, Min(0f)] private float _statsMultiplier;

        public WaveData(SpawnRequest regularEnemies, SpawnRequest onetimeEnemies, float statsMultiplier) {
            _regularEnemies = regularEnemies;
            _onetimeEnemies = onetimeEnemies;
            _statsMultiplier = statsMultiplier;
        }

        public SpawnRequest regularEnemies => _regularEnemies;
        public SpawnRequest onetimeEnemies => _onetimeEnemies;
        public float statsMultiplier => _statsMultiplier;

    }
}