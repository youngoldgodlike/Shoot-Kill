using System;
using _Shoot_Kill.Architecture.Scripts.Data;
using _Shoot_Kill.UI.Prefabs.StatisticWindow.Scripts;
using Assets.Architecture.Scripts.Utils;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using R3;

namespace Assets.Prefabs.Characters.MainHero.Scripts
{
    [Serializable]
    public class HeroData : IHaveStatisticText, IResettable
    {
        [field: Header("Properties")]
        [field: SerializeField, ReadOnly] public Vector3 direction { get; private set; }
        [field: SerializeField] public HeroHealthData healthData { get; private set; }
        [field: SerializeField] public FloatDataProperty moveSpeed { get; private set; }
        [field: SerializeField] public Cooldown dashDelay { get; private set; }
        [field: SerializeField] public float dashForce { get; private set; }

        public ReactiveProperty<float> defaultSpeed { get; private set; }

        private HeroConfig _config;

        public HeroData(HeroConfig config)
        {
            _config = config;
            
            Reset();
        }

        public void Reset()
        {
            moveSpeed = new FloatDataProperty(_config.moveSpeed.Value, _config.moveSpeed.maxValue);
            dashDelay = new Cooldown(_config.dashDelay.Value, _config.dashDelay.maxValue);
            healthData = new HeroHealthData(_config.health.Value);

            defaultSpeed = new ReactiveProperty<float>();
            defaultSpeed.Value = moveSpeed.Value;
            
            dashForce = _config.dashForce;
        }
        
        public async UniTaskVoid UseSlowdown(float timeSlowdown)
        {
            float timer = 0;
            
            while (timer <= timeSlowdown)
            {
                moveSpeed.Value = Mathf.Lerp(0, defaultSpeed.Value, timer / timeSlowdown);
                
                timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }
        
        public void SetDirection(Vector3 value)
            => direction = value;
        
        public void ReduceDashDelay(float percent)
        {
            dashDelay.delay.Value -= dashDelay.delay.Value / 100 * percent;

            if (dashDelay.delay.Value < 1f) 
                dashDelay.delay.Value = 1;
        }

        public void IncreaseMoveSpeed(float percent)
        {
            defaultSpeed.Value += defaultSpeed.Value / 100 * percent;
            moveSpeed.Value = defaultSpeed.Value;

            if (defaultSpeed.Value > 10)
            {
                defaultSpeed.Value = 10;
                moveSpeed.Value = defaultSpeed.Value;
            }
        }

        public void SetStatisticText(MatchStatisticController controller)
        {
            var moveSpeedText = controller.SpawnText();
            defaultSpeed.Subscribe(value => 
            { moveSpeedText.UpgradeInfo($"Скорость: {Mathf.Round(value)}"); });

            var dashDelayText = controller.SpawnText();
            dashDelay.delay.Subscribe(value => 
            { dashDelayText.UpgradeInfo($"Перезарядка рывка: {Math.Round(value, 2)} c."); });
        }
    }
}