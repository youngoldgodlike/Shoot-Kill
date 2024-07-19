using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Architecture.GameData;

namespace SpawnSystem.TestSpawner
{
    [Serializable]
    public struct SpawnRequest
    {
        public readonly SerializableGuid guid;
        public readonly CancellationTokenSource cts;
        
        public readonly SpawnType spawnType;
        public readonly List<EnemyPreset> enemyPresets;
        public readonly string identification;

        public SpawnRequest(IEnumerable<EnemyPreset> data, string identification,SpawnType spawnType) {
            enemyPresets = data.ToList();
            this.identification = identification;
            this.spawnType = spawnType;

            guid = SerializableGuid.NewGuid();
            cts = new CancellationTokenSource();
        }
    }
    
    public enum SpawnType
    {
        Regular,Onetime
    }
}