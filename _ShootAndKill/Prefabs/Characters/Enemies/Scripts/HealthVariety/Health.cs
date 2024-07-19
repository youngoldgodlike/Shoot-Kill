using Enemies.EnemiesVariety;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety
{
    public abstract class Health : MonoBehaviour
    {
        [Header("Base Dependencies")]
        [SerializeField] protected Animator _animator;
        [SerializeField, ReadOnly] protected float _maxHealth;
        [SerializeField, ProgressBar("Health", "_maxHealth", EColor.Red)]
        protected float _health;
        [SerializeField] protected Enemy _enemy;

        protected static readonly int Death = Animator.StringToHash("Death");
        
        public UnityEvent onHealthChange = new();
        public UnityEvent onZeroHealth = new();
        public UnityEvent onDieProcessEnd = new();
        public UnityEvent onPreDie = new();

        public float health => _health;
        public float maxHealth => _maxHealth;

        private void Start() {
            _enemy.onSpawn.AddListener(() => {
                _health = _maxHealth;
                onHealthChange.Invoke();
                _animator.SetBool(Death, false);
            });
        }

        protected abstract void OnZeroHealth();

        /// <summary>
        /// ONLY FOR ANIMATOR. THX
        /// </summary>
        public void Die() {
            onDieProcessEnd.Invoke();
        }

        public Health Initialize(float maxHp) {
            _maxHealth = maxHp;
            _health = _maxHealth;
            
            return this;
        }
        
        public void TakeDamage(float damage) {
            _health = Mathf.Clamp(_health - damage, 0, _maxHealth);
            onHealthChange.Invoke();

            if (_health == 0) {
                onZeroHealth?.Invoke();
                OnZeroHealth();
            }
        }

        [Button("KILL")]
        private void KillMe() {
            TakeDamage(_maxHealth);
        }
    }
}