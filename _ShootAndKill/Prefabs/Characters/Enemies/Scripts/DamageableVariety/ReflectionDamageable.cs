using System;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.DamageableVariety;
using Characters;
using UnityEngine;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts
{
    public class ReflectionDamageable : Damageable
    {
        [SerializeField, Min(0f)] private float _reflectionTime;
        [SerializeField, Min(0f)] private float _reflectionReloadTime;

        protected override void DamageTaking(IDamageSource source) {
            var damage = source.damage;
            
            if (damage < 0) {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }
            
            _health.TakeDamage(damage);
        }
    }
}