using System;
using _Shoot_Kill.Architecture.Scripts.Data;
using R3;
using UnityEngine;

namespace Assets.Prefabs.Characters.MainHero.Scripts
{
    [Serializable]
    public class HeroHealthData
    {
        [field: SerializeField] public FloatDataProperty health { get; private set; }

        public ReactiveProperty<float> maxHealth { get; private set; }

        public HeroHealthData(float value)
        {
            health = new FloatDataProperty(value, value);
            maxHealth = new ReactiveProperty<float>();
            
            maxHealth.Value = health.Value;
        }
        
        public void SetZeroHealth() => health.Value = 0;
        
        public void ChangeHeroHealth(float value)
        {
            if (value == 0) return;

            health.Value += value;
        }

        public void Heal()
        {
            health.Value += maxHealth.Value / 10;
            
            if (health.Value > maxHealth.Value)
                health.Value = maxHealth.Value;
        }

        public void IncreaseHealth(float value)
        {
            var currentPercent = health.Value * 100 / maxHealth.Value;
            maxHealth.Value += value;
            health.Value = currentPercent * maxHealth.Value / 100;
        }
    }
}