using Characters;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.Prefabs.Guns.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Projectile : MonoBehaviour, IDamageSource
    {
        [SerializeField, Min(0), ReadOnly] private float _damage = 10f;
        [field: SerializeField] public Rigidbody rigidBody { get; private set; }
        
        public float damage => _damage;
        public GameObject damageDealer => gameObject;

        private bool _abilityIsActive;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.DealDamage(this);
                
                if (_abilityIsActive) return;
                DisposeProjectile();
            }
            else
            {
                DisposeProjectile();
            }
        }

        protected void DisposeProjectile()
        {
            gameObject.SetActive(false);
            rigidBody.velocity = Vector3.zero;
        }

        public void Init(float value, bool abilityIsActive)
        {
            _damage = value;
            _abilityIsActive = abilityIsActive;
        }
    }
}
