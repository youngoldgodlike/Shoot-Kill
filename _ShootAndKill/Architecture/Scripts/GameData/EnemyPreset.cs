using System;
using Architecture.GameData.Configs;
using NaughtyAttributes;
using UnityEngine;

namespace Architecture.GameData
{
    [Serializable]
    public struct EnemyPreset
    {
        [field:Expandable] public EnemyConfig enemyConfig { get; private set; }
        [field:Min(0f)] public int requiredQuantity { get; private set; }
        [field:Min(0f)] public float spawnCooldown { get; private set; }
        [field:Min(1f)] public float statsMultiplier { get; private set; }
        [field:Min(1f)] public float expMultiplier { get; private set; }

        public SerializableGuid guid { get; }

        public EnemyPreset(EnemyConfig enemyConfig, int requiredQuantity = 1, float spawnCooldown = 1f,float statsMultiplier = 1f,float expMultiplier = 1f) {
            this.enemyConfig = enemyConfig;
            this.requiredQuantity = requiredQuantity;
            this.spawnCooldown = spawnCooldown;
            this.statsMultiplier = statsMultiplier;
            this.expMultiplier = expMultiplier;
            
            guid = SerializableGuid.NewGuid();
        }
    }
}