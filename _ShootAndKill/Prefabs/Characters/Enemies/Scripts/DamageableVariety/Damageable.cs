using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;
using Characters;
using Enemies.EnemiesVariety;
using UnityEngine;
using UnityEngine.Events;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.DamageableVariety
{
    [RequireComponent(typeof(Health))]
    public abstract class Damageable : MonoBehaviour, IDamageable
    {
        [SerializeField] protected Health _health;
        [SerializeField] protected Animator _animator;
        private Collider _collider;
        private static readonly int Hit = Animator.StringToHash("Hit");

        public UnityEvent onImpact;
        
        private void Awake() {
            if (!TryGetComponent(out _collider))
                _collider = GetComponentInChildren<Collider>();
            
            _health.onPreDie.AddListener(() => _collider.enabled = false);
            GetComponent<Enemy>().onSpawn.AddListener(() => _collider.enabled = true);
        }

        public void DealDamage(IDamageSource source) {
            _animator.SetTrigger(Hit);
            onImpact.Invoke();
            DamageTaking(source);
        }

        protected abstract void DamageTaking(IDamageSource source);

        public void Initialize(){}
    }
}