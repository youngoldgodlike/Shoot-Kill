using System;
using Characters;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.DamageableVariety
{
    public class DefaultDamageable : Damageable
    {
        protected override void DamageTaking(IDamageSource source) {
            var damage = source.damage;
            
            if (damage < 0) {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }
            
            _health.TakeDamage(damage);
        }
    }
}