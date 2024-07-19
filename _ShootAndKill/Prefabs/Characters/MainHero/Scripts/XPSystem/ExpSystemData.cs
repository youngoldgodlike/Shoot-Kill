using System;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Prefabs.Characters.MainHero.Scripts.XPSystem
{
    [Serializable]
    public class ExpSystemData : IResettable
    {
        [field: SerializeField] public float maxExp { get; private set; }
        
        [SerializeField] private int _growthFactor = 10;
        public ReactiveProperty<int> currentLevel { get; private set; }
        public ReactiveProperty<float> expProgress { get; private set; }

        public UnityEvent<int> onLevelUp = new();

        private ExpSystemConfig _expSystemConfig;

        public ExpSystemData(ExpSystemConfig config)
        {
            _expSystemConfig = config;
            
            Reset();
        }
        
        public void Reset()
        {
            expProgress = new ReactiveProperty<float>();
            currentLevel = new ReactiveProperty<int>();

            expProgress.Value = 0;
            currentLevel.Value = 1;
            
            maxExp = _expSystemConfig.maxExp;
            _growthFactor = _expSystemConfig.growthFactor;
        }

        public void AddExp(float exp)
        {
            expProgress.Value += exp;

            if (expProgress.Value >= maxExp)
            {
                currentLevel.Value++;
                expProgress.Value = (maxExp - expProgress.Value) * -1;
                maxExp += maxExp / 100 * _growthFactor;
                
                onLevelUp?.Invoke(currentLevel.Value);
            }
        }
    }
}