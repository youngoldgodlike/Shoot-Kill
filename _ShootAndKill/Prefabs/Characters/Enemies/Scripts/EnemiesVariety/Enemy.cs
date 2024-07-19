using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.Attachments;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;
using Enemies.DmgInfliction;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Motion = Enemies.MotionVariety.Motion;

namespace Enemies.EnemiesVariety
{
    [RequireComponent(typeof(DeathDrop))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField, ReadOnly] public Transform parent;
        //[ShowNativeProperty] private int guid => _guid.GetHashCode();
        [SerializeField, ReadOnly] private float _expQuantity;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private DeathDrop _deathDrop;
        [SerializeField] private Health _hp;
        [SerializeField] private DamageInfliction _damageInfliction;
        [SerializeField] private Motion _motion;
        [SerializeField] private NavMeshAgent _agent;

        public Health health => _hp;
        public DamageInfliction dmgInfliction => _damageInfliction;
        public Motion motion => _motion;
        public Collider collidr => _collider;
        public Rigidbody rigibody => _rigidbody;
        public NavMeshAgent agent => _agent;
        
        //private SerializableGuid _guid;
        public UnityEvent onSpawn = new();

        private void OnValidate() {
            parent = _collider.transform.root;
            Bootstrap();
        }

        private void Awake() {
            Bootstrap();
            _hp.onPreDie.AddListener(() => _deathDrop.DropExp(_expQuantity));
            //onSpawn.AddListener(() => _guid = SerializableGuid.NewGuid());
        }

        private void Bootstrap() {
            if (_collider.IsUnityNull()) TryGetComponent(out _collider);
            if (_hp.IsUnityNull()) TryGetComponent(out _hp);
            if (_damageInfliction.IsUnityNull()) TryGetComponent(out _damageInfliction);
            if (_motion.IsUnityNull()) TryGetComponent(out _motion);
            if (_deathDrop.IsUnityNull()) TryGetComponent(out _deathDrop);
            if (_rigidbody.IsUnityNull()) TryGetComponent(out _rigidbody);
            if (_agent.IsUnityNull()) parent.TryGetComponent(out _agent);
        }
        
        public void Initialize(float expQuantity) {
            _expQuantity = expQuantity;
        }
    }
}