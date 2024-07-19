using Enemies.EnemiesVariety;
using NaughtyAttributes;
using UnityEngine;

namespace Architecture.GameData.Configs
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField, ShowAssetPreview, Required] private GameObject _prefab;
        [SerializeField] private string _fullname = "Nameless enemy";
        [SerializeField, Min(0f)] private float _health, _damage, _attackDistance;
        [SerializeField] private float _stopDistance = 1f, _motionSpeed = 1f;
        [SerializeField,Min(0f)] private float _expQuantity;
        [SerializeField] private float _additionalPower;
        public SerializableGuid guid { get; } = SerializableGuid.NewGuid();
        [ShowNativeProperty] public float power => _health / 100 + _damage / 10 + _attackDistance / 10 + _additionalPower;

        public GameObject enemyPrefab => _prefab;
        public string fullname => _fullname;
        public float health => _health;
        public float damage => _damage;
        public float attackDistance => _attackDistance;
        public float stopDistance => _stopDistance;
        public float motionSpeed => _motionSpeed;
        public float expQuantity => _expQuantity;

        private void OnValidate() {
            if(_stopDistance == 0) return;
            _attackDistance = Mathf.Clamp(_attackDistance, _stopDistance + 0.5f, Mathf.Infinity);
        }
    }
}