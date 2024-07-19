using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public abstract class SpawnPositionFinder : MonoBehaviour
    {
        [SerializeField] protected Transform player;

        public abstract Vector3 GetSpawnPos();
    }
}