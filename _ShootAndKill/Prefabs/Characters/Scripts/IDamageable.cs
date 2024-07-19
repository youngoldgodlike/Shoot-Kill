using UnityEngine;

namespace Characters
{
    public interface IDamageable
    {
        public void DealDamage(IDamageSource source);
    }

    public interface IDamageSource
    {
        public float damage { get; }
        public GameObject damageDealer { get; }
    }
    
    public class EnemyDamageDealer : IDamageSource
    {
        public EnemyDamageDealer(float dmg,GameObject damageDealer) {
            damage = dmg;
            this.damageDealer = damageDealer;
        }

        public void SetDamage(float dmg) => damage = dmg;
        public void SetDealer(GameObject dealer) => damageDealer = dealer;

        public float damage { get; private set; }

        public GameObject damageDealer { get; private set; }
    }
}