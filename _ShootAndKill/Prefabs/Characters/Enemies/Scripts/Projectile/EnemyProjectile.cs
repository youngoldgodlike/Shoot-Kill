using _Shoot_Kill.Architecture.Scripts.Utilities;
using Characters;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class EnemyProjectile : MonoBehaviour, IDamageSource
    {
        [SerializeField, Min(0f)] private float _lifeTime;
        [SerializeField, ReadOnly] private float _damage;
        private CountdownTimer _timer;
        private float _speed;
        private Vector3 _direction;

        private Vector3 position {
            get => transform.position;
            set => transform.position = value;
        }

        public float damage => _damage;
        public GameObject damageDealer => gameObject;

        public UnityEvent onHit;

        private void Update() {
            _timer.Tick(Time.deltaTime);
            transform.Translate(_direction * (_speed * Time.deltaTime),Space.World);
        }

        public void Initialize(Vector3 target, float speed,Vector3 spawnPos, float dmg) {
            position = spawnPos;
            _direction = Vector3.ProjectOnPlane((target - position).normalized, Vector3.up);
            _speed = speed;
            _damage = dmg;

            target.y = position.y;
            transform.LookAt(target);
            
            _timer = new CountdownTimer(_lifeTime);
            _timer.OnTimerStop += () => onHit.Invoke();
            _timer.Start();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out IDamageable damageTaker)) {
                damageTaker.DealDamage(this);
                onHit.Invoke();
            }
            else {
                onHit.Invoke();
            }
        }
    }
}
