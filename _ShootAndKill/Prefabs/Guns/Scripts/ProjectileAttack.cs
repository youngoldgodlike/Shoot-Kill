using System;
using _Shoot_Kill.Architecture.Scripts.Data;
using _Shoot_Kill.UI.Prefabs.StatisticWindow.Scripts;
using Assets.Architecture.Scripts.Utils;
using Assets.Prefabs.Characters.MainHero.Scripts;
using Assets.UI.Architecture.Scripts;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Prefabs.Guns.Scripts
{
    [RequireComponent(typeof(PlaySoundsComponent))]
    public class ProjectileAttack : MonoBehaviour, IHaveStatisticText, IResettable
    {
        [SerializeField] private ProjectileGunConfig _config;
        [SerializeField] private ProgressBarWithText _storeProgressBar;
        [SerializeField] private Transform _weaponMuzzle;
        [SerializeField] private ParticleSystem _muzzleVfx;
        [SerializeField] private Renderer[] _renderers;

        [Header("Properties")]
        [SerializeField, Min(0f)] private float _force = 15f;

        [SerializeField] private ForceMode _forceMode = ForceMode.Impulse;

        [field: SerializeField, ReadOnly] public IntDataProperty storeCount { get; private set; }
        [field: SerializeField, ReadOnly] public FloatDataProperty damage { get; private set; }
        [field: SerializeField, ReadOnly] public FloatDataProperty reloadSpeed { get; private set; }
        [field: SerializeField, ReadOnly] public Cooldown rate { get; private set; }

        private Image _reloadAbilityBar;
        private PlaySoundsComponent _sounds;
        private ProjectilePool _pool;
        private Animator _animator;

        public int maxStoreCount { get; private set; }
        
        private float _defaultDamage;
        private float _defaultRateInSecond;
        private float _totalRateInSec;
        private float _abilityTime;

        private bool _isReload;
        private bool _abilityIsActive;
        private bool _abilityIsReload;
        
        private static readonly int IsAttack = Animator.StringToHash("isAttack");

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _sounds = GetComponent<PlaySoundsComponent>();
            
            Reset();
        }

        private void Start()
        {
            storeCount.Subscribe(value => SetViewProgress());
        }

        public void Init( ProjectilePool pool, ProgressBarWithText barWithText, Image reloadAbilityBar)
        {
            _pool = pool;
            _storeProgressBar = barWithText;
            _reloadAbilityBar = reloadAbilityBar;
            
            SetViewProgress();
        }

        public void Reset()
        {
            storeCount = new IntDataProperty(_config.storeCount.Value, _config.storeCount.maxValue);
            reloadSpeed = new FloatDataProperty(_config.reloadSpeed.Value, _config.reloadSpeed.maxValue);
            damage = new FloatDataProperty(_config.damage.Value, _config.damage.maxValue);
            rate = new Cooldown(_config.rate.Value, _config.rate.maxValue);

            _defaultDamage = damage.Value;
            maxStoreCount = storeCount.Value;
            _defaultRateInSecond = 1 / rate.delay.Value;
            _totalRateInSec = _defaultRateInSecond;
            _abilityTime = _config.abilityTime;
        }

        [Button("PerformAttack")]
        public void PerformAttack()
        {
            if (!rate.isReady || _isReload) return;

            if (storeCount.Value > 0 )
            {
                Attack();
                rate.Reset();

                if (storeCount.Value <= 0)
                    Reload().Forget();
            }
            else
                Reload().Forget();
        }

        private void Attack()
        {
            _animator.SetTrigger(IsAttack);
            _muzzleVfx.Play();
            _sounds.Play();
            
            var projectile = _pool.GetObject(false);
            projectile.transform.rotation = transform.rotation;
            projectile.transform.position = _weaponMuzzle.position;

            projectile.Init(damage.Value, _abilityIsActive);
            projectile.gameObject.SetActive(true);

            projectile.rigidBody.AddForce(_weaponMuzzle.forward * _force, _forceMode);

            storeCount.Value--;
        }

        public async UniTaskVoid UseAbility()
        {
            if (_abilityIsActive || _abilityIsReload) return;

            _abilityIsActive = true;

            foreach (var renderer in _renderers)
                renderer.material.color = Color.red;

            float time = _abilityTime;
            while (time >= 0 )
            {
                time -= Time.deltaTime;

                _reloadAbilityBar.fillAmount = time / _abilityTime;
                await UniTask.Yield();
            }

            foreach (var renderer in _renderers)
                renderer.material.color = Color.white;
            _abilityIsActive = false;
            
            ReloadAbility().Forget();
        }

        private async UniTaskVoid ReloadAbility()
        {
            _abilityIsReload = true;

            float time = 0;
            
            while (time < 10 )
            {
                time += Time.deltaTime;
                
                _reloadAbilityBar.fillAmount = time / 10;
                await UniTask.Yield();
            }

            _abilityIsReload = false;
        }


        public async UniTaskVoid Reload()
        {
            if (storeCount.Value >= maxStoreCount || _isReload) return;
                
            _isReload = true;
            _storeProgressBar.StartCountdown(reloadSpeed.Value).Forget();
            
            await UniTask.Delay(TimeSpan.FromSeconds(reloadSpeed.Value));
            
            storeCount.Value = maxStoreCount;
            _isReload = false;
        }

        public void IncreaseGunDamage(float percent) =>
            damage.Value += _defaultDamage / 100 * percent;

        public void ReduceReloadSpeed(float percent) =>
            reloadSpeed.Value -= reloadSpeed.Value / 100 * percent;
        
        public void IncreaseFireRate(float percent)
        {
          var needRateToIncrease = _defaultRateInSecond / 100 * percent;
          _totalRateInSec += needRateToIncrease;

          rate.delay.Value = 1 / _totalRateInSec;
          
          if (rate.delay.Value < 0.05f)
              rate.delay.Value = 0.05f;
        }

        public void IncreaseStoreCount(int value)
        {
            storeCount.Value += value;
            maxStoreCount += value;
            
            SetViewProgress();
        }

        public void SetStatisticText(MatchStatisticController controller)
        {
            var damageText = controller.SpawnText();
            damage.Subscribe(value =>
            {
                damageText.UpgradeInfo($"Урон: {Convert.ToInt32(value)}");
            });
            
            var rateText = controller.SpawnText();
            rate.delay.Subscribe(value =>
            {
                rateText.UpgradeInfo($"Скорость стрельбы: {Math.Round(value, 2)} с.");
            });

            var reloadSpeedText = controller.SpawnText();
            reloadSpeed.Subscribe(value => 
            {
                reloadSpeedText.UpgradeInfo($"Скорость перезарядки: {Math.Round(value, 2)} с.");
            });
        }

        private void SetViewProgress() => _storeProgressBar
            .SetProgressWithText(storeCount.Value, maxStoreCount, 
                $"{storeCount.Value}/{maxStoreCount}");
    }
}
