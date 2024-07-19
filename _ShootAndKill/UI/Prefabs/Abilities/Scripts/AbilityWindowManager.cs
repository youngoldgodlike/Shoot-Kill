using System;
using System.Collections.Generic;
using _Shoot_Kill.Architecture.Scripts;
using TMPro;
using UnityEngine;
using YG;
using Zenject;
using Random = System.Random;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class AbilityWindowManager : Singleton<AbilityWindowManager>
    {
        [SerializeField] private MatchStatisticController _statisticController;
        [SerializeField] private Transform _abilityContainer;
        [SerializeField] private Transform _window;
        [SerializeField] private Transform _descriptionWindow;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private RefreshButton _refreshButton;
        [SerializeField] private ActiveButton _confirmButton;
        [SerializeField] private AbilityDataStorage _abilityDataStorage;
        [SerializeField] private AbilityRarityDataScriptableObject _abilityRarityData;
        
        [Space] [Header("Properties")]
        [SerializeField] private int _startRefreshCount = 3;
        [Header("ChanceSpawnAbilities")]
        [SerializeField, Range(0, 100)] private int _commonChance = 50;
        [SerializeField, Range(0, 100)] private int _rareChance = 35;
        [SerializeField, Range(0, 100)] private int _mythicalChance = 15;
        
        private List<AbilData> _abilityData;
        private Dictionary<AbilityTag, AbilityCell> _currentAbilityCells;
        private AbilityCell _selectedAbility;
        private GameSession _gameSession;

        public bool isOpenAbilityWindow { get; private set; }
        private const int _adRewId = 1;
        private int _currentRefreshCount = 3;
        private int _upLevelCount = 0;

        [Inject]
        private void Initialize(GameSession gameSession)
        {
            _gameSession = gameSession;
        }

        protected override void Awake()
        {
            base.Awake();
            
            _abilityData = _abilityDataStorage.abilitiesData;
            
            _currentRefreshCount = _startRefreshCount;
            _currentAbilityCells = new Dictionary<AbilityTag, AbilityCell>();
            
            SetActiveRefreshButton();
        }

        private void Start()
        {
            _gameSession.matchData.expSystemData.onLevelUp.AddListener(TryShowAbility);
            YandexGame.RewardVideoEvent += AddRewardedLevel;
        }

        private void OnDisable() =>
            YandexGame.RewardVideoEvent -= AddRewardedLevel;

        public void ChoiceAbility(AbilityCell selected)
        {
            _confirmButton.SetActive(true);
            _selectedAbility = selected;
            
            foreach (var ability in _currentAbilityCells)
                ability.Value.Hide();
            
            _selectedAbility.Show();
        }

        private void CalculateLots()
        {
            Random random = new Random();
            int progressCount = 0;
            
            while (progressCount < 3)
            {
                var data = _abilityData[random.Next(0, _abilityData.Count)];
                
                if (CheckRepeatAbility(data.Tag)) continue;
                
                var chance = random.Next(0, 100);

                if (chance <= _mythicalChance )
                    SpawnAbility(data, AbilityRarity.Mythical);
                else if (chance <= _rareChance)
                    SpawnAbility(data, AbilityRarity.Rare);
                else if (chance <= _commonChance)
                    SpawnAbility(data, AbilityRarity.Common);
                
                progressCount++;
            }
        }

        private void SpawnAbility(AbilData cell, AbilityRarity rarity)
        {
            var ability = Instantiate(cell.Ability, _abilityContainer);
            ability.Initialize(_gameSession);
                
            ability.onClick.AddListener( () => ChoiceAbility(ability));
            ability.onPointerEnter.AddListener(text  => ShowDescription(text));
            ability.onPointerExit.AddListener(() => HideDescription());
            ability.OnUpgrade.AddListener( _statisticController.UpdateStatistic); 
            ability.SetRarity(rarity, _abilityRarityData );
                
            _currentAbilityCells.Add(cell.Tag, ability);
        }

        private void ShowDescription(string message)
        {
            _descriptionWindow.gameObject.SetActive(true);
            _descriptionText.text = message;
        }

        private void HideDescription()
        {
            _descriptionWindow.gameObject.SetActive(false);
        }

        public void Confirm()
        {
            if (_selectedAbility == null) return;
            
            _selectedAbility.Upgrade();

            ClearCurrentAbility();

            _gameSession.UIIsActive.Value = false;
            _window.gameObject.SetActive(false);
            HideDescription();
            Time.timeScale = 1f;

            _upLevelCount--;
            isOpenAbilityWindow = false;
            
            if (_upLevelCount != 0)
                ShowAbility();
        }

        private void ClearCurrentAbility()
        {
            foreach (var ability in _currentAbilityCells)
                Destroy(ability.Value.gameObject);
            
            _currentAbilityCells.Clear();
        }

        public void RefreshAbility()
        {
            if (_currentRefreshCount <= 0) return;
            
            ClearCurrentAbility();
            CalculateLots();

            _confirmButton.SetActive(false);
            _selectedAbility = null;
            
            _currentRefreshCount--;

            SetActiveRefreshButton();
        }

        private void TryShowAbility(int currentLevel)
        {
            _upLevelCount++;
            
            if (currentLevel % 5 == 0)
            {
                _currentRefreshCount += _startRefreshCount;
                SetActiveRefreshButton();
            }

            if (_upLevelCount > 1) return;
            
            ShowAbility();
        }

        private void ShowAbility()
        {
            if (isOpenAbilityWindow) return;
            
            _gameSession.UIIsActive.Value = true;
            _window.gameObject.SetActive(true);
            _confirmButton.SetActive(false);
            Time.timeScale = 0f;

            isOpenAbilityWindow = true;
            
            CalculateLots();
        }

        private void SetActiveRefreshButton()
        {
            var isActive = _currentRefreshCount > 0;
            _refreshButton.SetActiveAndText(isActive,$"x{_currentRefreshCount}");
        }

        private bool CheckRepeatAbility(AbilityTag tag)
        {
            if (_currentAbilityCells.Count == 0) return false;
            
            foreach (var ability in _currentAbilityCells)
            {
                if (ability.Key == tag)
                    return true;
            }
            
            return false;
        }

        public void OnClickAdButton() =>
            YGAdHandler.ShowRewardedAd(_adRewId);

        private void AddRewardedLevel(int id)
        {
            if (_adRewId != id) return;

            _upLevelCount += 3;
            
            ShowAbility();
        }
    }
}
