using System;
using _Shoot_Kill.Architecture.Scripts;
using Assets.UI.Architecture.Scripts;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.Events;
using Zenject;

namespace Assets.Prefabs.Characters.MainHero.Scripts.XPSystem
{
    public class ExpSystem : Singleton<ExpSystem>
    {
        [SerializeField] private ProgressBarWithText _bar;
        [SerializeField] private TextMeshProUGUI _currentLevelText;

        public UnityEvent onReceiveExp;
        
        private PlaySoundsComponent _playSoundsComponent;
        
        private ExpSystemData expSystemData => _gameSession.matchData.expSystemData;
        
        private GameSession _gameSession;

        [Inject]
        private void Initialize(GameSession gameSession)
        {
            _gameSession = gameSession;
        }
        
        protected void Start()
        {
            expSystemData.expProgress.Subscribe(_ =>
            {
                _bar.SetProgressWithText(expSystemData.expProgress.Value, expSystemData.maxExp,
                    $"{Convert.ToInt32(expSystemData.expProgress.Value)}/{Convert.ToInt32(expSystemData.maxExp)}");
            });
            
            expSystemData.currentLevel.Subscribe(value =>
            { _currentLevelText.text = $"Уровень: {value}"; });

            _playSoundsComponent = GetComponent<PlaySoundsComponent>();
            
            expSystemData.onLevelUp.AddListener( _ => _playSoundsComponent.Play());
        }
        

        [Button("Level Up")]
        public void LevelUp()
            => expSystemData.AddExp(expSystemData.maxExp);
        
        public void ReceiveExp(float quantity)
        {
            expSystemData.AddExp(quantity);
            onReceiveExp?.Invoke();
        }
    }
}
