using System;
using _Shoot_Kill.Architecture.Scripts;
using Assets.UI.Architecture.Scripts;
using Characters;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using R3;
using Zenject;

namespace Assets.Prefabs.Characters.MainHero.Scripts
{  
    public class HeroHealth : Singleton<HeroHealth>, IDamageable
    {
        [SerializeField] private ProgressBarWithText _bar;
        [SerializeField] private Image _hitImage;
        [SerializeField] private Color _defaultHitImageColor;
        [SerializeField] private Color _hitColor;
        [SerializeField] private bool _godMode;
        
        public UnityEvent onDamage;
        public UnityEvent onHeal;
        public UnityEvent onDie;

        private HeroHealthData healthData =>
            _gameSession.matchData.heroData.healthData; 
       
        private const float _rednessTimer = 0.2f;
        public bool isDie =>
            healthData.health.Value <= 0;
        public bool isFullHealth =>
            healthData.health.Value >= healthData.maxHealth.Value;

        private GameSession _gameSession;

        [Inject]
        private void Initialize(GameSession gameSession) =>
            _gameSession = gameSession;
        
        private void Start()
        {
            healthData.health.Subscribe(_ => SetViewProgress());
#if !UNITY_EDITOR
            _godMode = false;
#endif
        }

        public void DealDamage(IDamageSource source)
        {
            if(_godMode) return;
            
            healthData.ChangeHeroHealth(-source.damage);
            
            OnAttackRedness().Forget();
            onDamage?.Invoke();
            
            if (!isDie) return;

            healthData.SetZeroHealth();
            
            onDie?.Invoke();
        }

        public void Heal(float value)
        {
            if (isFullHealth) return;
            
            healthData.Heal();
            
            onHeal?.Invoke();
        }

        [Button("Hit")]
        private void TestDie()
        {
            healthData.ChangeHeroHealth(-10);
           
            
            OnAttackRedness().Forget();
            onDamage?.Invoke();
            
            if (!isDie) return;

            healthData.SetZeroHealth();
            
            onDie?.Invoke();
        }

        private async UniTaskVoid OnAttackRedness()
        {
            float time = 0f;

            while (time < _rednessTimer)
            {
                time += Time.deltaTime;

                _hitImage.color = Color.Lerp(_defaultHitImageColor, _hitColor, time / _rednessTimer);

                await UniTask.Yield();
            }
            
            _hitImage.color = _defaultHitImageColor;
        }
        
        private void SetViewProgress() => _bar.SetProgressWithText(
            healthData.health.Value , healthData.maxHealth.Value,
            $"{Convert.ToInt32(healthData.health.Value)} / {Convert.ToInt32(healthData.maxHealth.Value)}");
    }
}
