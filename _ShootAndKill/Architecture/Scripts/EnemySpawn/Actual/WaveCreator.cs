using System;
using System.Collections.Generic;
using System.Linq;
using _Shoot_Kill.Architecture.Scripts.Utilities;
using Architecture.GameData;
using Architecture.GameData.Configs;
using Assets.Prefabs.Characters.MainHero.Scripts;
using Assets.Prefabs.Guns.Scripts;
using Helpers.Debugging;
using NaughtyAttributes;
using SpawnSystem.TestSpawner;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace SpawnSystem
{
    public class WaveCreator : MonoBehaviour
    {
        [SerializeField] private GameLogger _logger;
        
        [SerializeField, Expandable] private List<EnemyConfig> _normal = new();
        [SerializeField, Expandable] private List<EnemyConfig> _elite = new();
        [SerializeField, Expandable] private List<EnemyConfig> _eliteRegular = new();
        [SerializeField, Expandable] private List<EnemyConfig> _boss = new();
        [SerializeField, ReadOnly] private List<SimpleEnemyPreset> _commonPreset;
        [SerializeField, ReadOnly] private List<SimpleEnemyPreset> _elitePreset;

        [SerializeField, Range(1f, 2f)] private float _difficult = 1f;
        [SerializeField, Range(0f, 1f)] private float _eliteEnemiesCoef = 0.1f;
        [ShowNativeProperty] private float timerSecs => _matchTimer.IsUnityNull() ? 0f : _matchTimer.time;

        private float lowerBuff => Mathf.Lerp(2f, 1f, _difficult - 1);
        private float higherBuff => Mathf.Lerp(0.5f, 1f, _difficult - 1);

        [ShowNativeProperty] private float wavePowerMultiplier {
            get {
                // var b1 = 2f; i think i'm gonna need that for better balance
                var q = Mathf.Lerp(1.001f, 1.01f, _difficult - 1);
                var myQ = Mathf.Pow(q, (int)heroPower);
                return myQ;
            }
        }
        [ShowNativeProperty] private float wavePower => heroPower * wavePowerMultiplier * _difficult;
        [SerializeField] private int _startEnemyQuantity = 20;
        [SerializeField] private int _maxEnemyQuantity = 100;
        [SerializeField] private int _minutesToMaxQuantity;
        [ShowNativeProperty] private int addEnemyCooldown => _minutesToMaxQuantity * 60 / (_maxEnemyQuantity - _startEnemyQuantity);
        [ShowNativeProperty] private int enemiesCount{
            get {
                var currentCount = _startEnemyQuantity + Mathf.Abs(timerSecs / addEnemyCooldown) * wavePowerMultiplier;
                return Mathf.RoundToInt(Mathf.Clamp(currentCount, _startEnemyQuantity, _maxEnemyQuantity));
                //return Mathf.RoundToInt(heroPower / 2 + _timerSecs / 60);
            }
        }
        
        [SerializeField] private bool _showDebug;
        [SerializeField, ReadOnly, ResizableTextArea, ShowIf(nameof(_showDebug))] private string _debug;

        #region Stats
        
        private HeroData _heroData;
        private ProjectileAttack _gunData;

        
        [ShowNativeProperty] private float hp =>  _heroData.healthData.maxHealth.CurrentValue;
        [ShowNativeProperty] private float speed => _heroData.defaultSpeed.Value;


        [ShowNativeProperty] private float weaponDamage => _gunData.damage.Value;
        [ShowNativeProperty] private float weaponShootSpeed => 1 / _gunData.rate.delay.Value;
        [ShowNativeProperty] private float weaponReloadTimeInSec => _gunData.reloadSpeed.Value;
        [ShowNativeProperty] private float weaponMagazineCapacity => _gunData.maxStoreCount;
        [ShowNativeProperty] private float weaponDps => weaponDamage * weaponShootSpeed;
        [ShowNativeProperty] private float shootDuration => weaponMagazineCapacity / weaponShootSpeed;
        [ShowNativeProperty] private float shootCycle => shootDuration + weaponReloadTimeInSec;
        [ShowNativeProperty] private float weaponEfficiency => shootDuration / shootCycle;
        [ShowNativeProperty] private float weaponPower => weaponDps * weaponEfficiency;
        [ShowNativeProperty] private float heroPower => hp / 10 + speed + weaponPower;

        #endregion

        private int creationId;
        private Stopwatch _matchTimer;

        [Inject]
        private void InitializeZenject(GameSession gameSession) {
            _matchTimer = gameSession.matchData.stopwatchMatch;
            Initialize(gameSession.matchData.heroData, gameSession.matchData.currentGun);
        }
        
        public void Initialize(HeroData heroData,ProjectileAttack gunData) {
            _logger.WarningLog($"<color=blue>Инициализация скрипта WaveCreator</color>.");
            _heroData = heroData;
            _gunData = gunData;
            _logger.Log($"NULL CKECK. HERO DATA: {_heroData.IsUnityNull()}, GUN DATA: {_gunData.IsUnityNull()}");
            _logger.Log(
                $"HP:{hp}, SPD:{speed}, WD:{weaponDamage}, WSS:{weaponShootSpeed}, WRTIC:{weaponReloadTimeInSec}, WMC:{weaponMagazineCapacity}");
        }
        
        public WaveData CreateSpawnRequest(float waveDuration) {
            Clear();
            CreateWavePreset();
            
            var enemiesPowerSum = 0f;
            _commonPreset.ForEach(enemy => enemiesPowerSum += enemy.config.power * enemy.count);

            // Расчет множителя статов для врагов
            var statsMultiplier = wavePower / enemiesPowerSum;

            // ВЫЧИСЛЕНИЯ ДЛЯ ОПРЕДЕЛЕНИЯ КД СПАВНА
            float SpawnCD(int enemyCount,float power) {
                var baseCd = waveDuration / (enemyCount + 1) / wavePowerMultiplier;
                return Mathf.Clamp(baseCd, 1f, power * 2);
            }
            
            // Создание вражеских пресетов
            List<EnemyPreset> commonPresets = new();
            _commonPreset.ForEach(enemy =>
                commonPresets.Add(new EnemyPreset(enemy.config, enemy.count,
                    SpawnCD(enemy.count, enemy.config.power), statsMultiplier, wavePowerMultiplier)));
            
            List<EnemyPreset> elitePresets = new();
            _elitePreset.ForEach(enemy =>
                elitePresets.Add(new EnemyPreset(enemy.config, enemy.count,
                    SpawnCD(enemy.count, enemy.config.power), statsMultiplier,wavePowerMultiplier)));

            //Создание и отправка запроса
            var regularRequest = new SpawnRequest(commonPresets, $"Wave {creationId}:Regular", SpawnType.Regular);
            var onetimeRequest = new SpawnRequest(elitePresets, $"Wave {creationId}:Onetime", SpawnType.Onetime);
            creationId++;

            var request = new WaveData(regularRequest, onetimeRequest, statsMultiplier);

            _logger.Log($"Запрос создан. Количество врагов: {enemiesCount}, Сила врагов: {enemiesPowerSum}, Множитель статов: {statsMultiplier}", this);
           
            return request;
        }

        [Button("Create Wave Preset")]
        private void CreateWavePreset() {
            _logger.Log("Создание пресета врагов для волны...");
            
            CreateCommonEnemies();

            var enemiesPowerSum = 0f;
            _commonPreset.ForEach(enemy => enemiesPowerSum += enemy.config.power * enemy.count);
            
            CreateEliteEnemies(enemiesPowerSum);
            CreateEliteRegularEnemies(enemiesPowerSum);
        }

        private void CreateCommonEnemies() {
            // Вычисление коэфициента для определения количества каждого врага
            _commonPreset.Clear();
            var allPower = 0f;
            var log = "";

            _normal.ForEach(data => allPower += data.power);
            var baseCoefSum = 0f;
            var enemiesCoefs = new Dictionary<EnemyConfig, float>();

            foreach (var enemy in _normal) {
                var baseCoef = enemy.power / allPower;
                log += $"{enemy.fullname}:\nБазовый коэф основанный на базовой силе: {baseCoef}";
                baseCoefSum += baseCoef;
                log += $"\nСумма базовых коэфов: {baseCoefSum}";
                var coef = baseCoef * Mathf.Lerp(lowerBuff, higherBuff, baseCoefSum);
                log += $"\nОтрегулированный коэф: {coef}\n";

                try {
                    enemiesCoefs.Add(enemy, coef);
                }
                catch (Exception e) {
                    Debug.LogError(e);
                }
            }

            var coefsSum = enemiesCoefs.Sum(coef => coef.Value);

            // Определение итогового количества каждого обычного врага
            foreach (var enemy in _normal) {
                var quantity = Mathf.RoundToInt(enemiesCount * enemiesCoefs[enemy] / coefsSum);
                _commonPreset.Add(new SimpleEnemyPreset(quantity, enemy));
            }
        }

        private void CreateEliteRegularEnemies(float enemiesPowerSum) {
            var elitePower = enemiesPowerSum * _eliteEnemiesCoef;
            if (_eliteRegular.Count(enemy => enemy.power < elitePower) == 0) return;
            
            var availableEliteReg = new List<EnemyConfig>();
            var chosenEliteReg = new Dictionary<EnemyConfig, int>();
            
            foreach (var enemy in _eliteRegular.Where(enemy => enemy.power < elitePower)) {
                chosenEliteReg.Add(enemy, 0);
                availableEliteReg.Add(enemy);
                _debug += $"\n{enemy.fullname} Добавлен в возможный пул регулярных элитных";
            }
            
            var elitePowerSum = 0f;
            do {
                var randomId = Random.Range(0, availableEliteReg.Count);

                var enemy = availableEliteReg[randomId];
                chosenEliteReg[enemy] += 1;

                elitePowerSum += enemy.power;
                _debug += $"\nRandom id: {randomId}. Add {enemy.fullname}";
            } while (elitePowerSum < elitePower);
            
            foreach (var enemy in chosenEliteReg.Where(enemy => enemy.Value > 0)) {
                _commonPreset.Add(new SimpleEnemyPreset(enemy.Value, enemy.Key));
            }
        }
        private void CreateEliteEnemies(float enemiesPowerSum) {
            var elitePower = enemiesPowerSum * _eliteEnemiesCoef;
            if (_elite.Count(enemy => enemy.power < elitePower) == 0) {
                _debug += $"\nНет элитных врагов удовлетворяющих условию";

                return;
            }

            // разделение элитных врагов на регулярных и одноразовых
            _debug += $"\nДобавление элитных врагов\n";
            var availableElite = new List<EnemyConfig>();
            var chosenElite = new Dictionary<EnemyConfig, int>();

            foreach (var enemy in _elite.Where(enemy => enemy.power < elitePower)) {
                chosenElite.Add(enemy, 0);
                availableElite.Add(enemy);
                _debug += $"\n{enemy.fullname} Добавлен в возможный пул";
            }

            _debug += $"\n";

            var elitePowerSum = 0f;
            do {
                var randomId = Random.Range(0, availableElite.Count);

                var enemy = availableElite[randomId];
                chosenElite[enemy] += 1;

                elitePowerSum += enemy.power;
                _debug += $"\nRandom id: {randomId}. Add {enemy.fullname}";
            } while (elitePowerSum < elitePower);

            foreach (var enemy in chosenElite.Where(enemy => enemy.Value > 0)) {
                _elitePreset.Add(new SimpleEnemyPreset(enemy.Value, enemy.Key));
            }
        }

        public SpawnRequest CreateBoss() {
            var preset = new List<EnemyPreset> { new(_boss[0], statsMultiplier: wavePowerMultiplier) };
            var request = new SpawnRequest(preset, "Boss", SpawnType.Onetime);
            return request;
        }

        private void Clear() {
            _commonPreset.Clear();
            _elitePreset.Clear();
        }

        [Serializable]
        private struct SimpleEnemyPreset
        {
            public EnemyConfig config;
            public int count;

            public SimpleEnemyPreset(int count, EnemyConfig config) {
                this.config = config;
                this.count = count;
            }
        }
    }
}
