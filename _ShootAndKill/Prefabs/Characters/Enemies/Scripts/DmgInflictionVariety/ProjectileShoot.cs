using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts;
using Architecture.MinePool;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies.DmgInfliction
{
    public class ProjectileShoot : DamageInfliction
    {
        [Header("Projectile Settings")]
        [SerializeField, InspectorName("Projectile")] private EnemyProjectile _prefab;
        [SerializeField, InspectorName("Projectile speed")] private float _speed;
        [SerializeField, InspectorName("Projectile Spawn Pos")] private Transform _spawnPos;
        [SerializeField] private int _poolCapacity;
        [SerializeField, ReadOnly] private Transform _storage;
        private ExpandablePool<EnemyProjectile> _pool;

        private void Awake() {
            CreatePool();
        }

        private void CreatePool() {
            var storage = new GameObject($"{_prefab.name} storage");
            _storage = storage.transform;
            _pool = new ExpandablePool<EnemyProjectile>(Create, ReturnToPool, GetFromPool, _poolCapacity);
        }

        private EnemyProjectile Create() {
            var projectile = Instantiate(_prefab, _storage);
            projectile.onHit.AddListener(() => _pool.Return(projectile));
            
            return projectile;
        }

        private void GetFromPool(EnemyProjectile projectile) => projectile.gameObject.SetActive(true);

        private void ReturnToPool(EnemyProjectile projectile) => projectile.gameObject.SetActive(false);

        public override void Assault() {
            var projectile = _pool.Get();
            projectile.Initialize(target.position, _speed, _spawnPos.position,_damage);
        }

        private void OnDestroy() {
            if(!_storage.IsUnityNull()) Destroy(_storage.gameObject);
        }
    }
}