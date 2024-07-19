using System;
using System.Threading;
using _Shoot_Kill.Architecture.Scripts.Utilities;
using Assets.Prefabs.Characters.MainHero.Scripts;
using Assets.Prefabs.Characters.MainHero.Scripts.XPSystem;
using Assets.Prefabs.Guns.Scripts;
using Cysharp.Threading.Tasks;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;
using Zenject;

public class MatchData : MonoBehaviour, IResettable
{
    [field: SerializeField] public HeroData heroData { get; private set; }
    [field: SerializeField] public ExpSystemData expSystemData { get; private set; }
    [field: SerializeField] public WeaponData currentWeaponData { get; private set; }
    [field: SerializeField] public ProjectileAttack currentGun { get; private set; }
    
    public Stopwatch stopwatchMatch { get; private set; }
    private GameSession _gameSession;
    private CancellationTokenSource _tokenSource = new();

    [Inject]
    public void Initialize(HeroConfig heroConfig, ExpSystemConfig expSystemConfig, GameSession gameSession)
    {
        _gameSession = gameSession;
        heroData = new HeroData(heroConfig);
        expSystemData = new ExpSystemData(expSystemConfig);
        stopwatchMatch = new Stopwatch();
    }

    private void Awake()
    {
        _gameSession.UIIsActive.Where(x => !x).Subscribe(_ =>
        { StartMatchStopwatch();});

        _gameSession.UIIsActive.Where(x => x).Subscribe(_ =>
        { PauseStopwatch();});
        
        stopwatchMatch.Stop();
    }

    public void SelectGun(WeaponData data) =>
        currentWeaponData = data;

    public void SetProjectileAttack(ProjectileAttack projectileAttack) =>
        currentGun = projectileAttack;
    
    public void StartMatchStopwatch() =>
        stopwatchMatch.StartStopwatch(_tokenSource.Token).Forget();
    
    private void PauseStopwatch() =>
        stopwatchMatch.Pause();

    public void StopStopwatch() =>
        stopwatchMatch.Stop();

    public void OnChangedScene(Scene scene) =>
        Reset();
    

    public void Reset()
    {
        _tokenSource.Cancel();
        _tokenSource = new CancellationTokenSource();
        
        heroData.Reset(); 
        expSystemData.Reset();
        stopwatchMatch.Stop();
    }

    public void CheckBetterTime(Action<float> callBack = null)
    {
        callBack?.Invoke(stopwatchMatch.time);
        
        if (YandexGame.savesData.betterMatchTime <= stopwatchMatch.time)
        {
            YandexGame.savesData.betterMatchTime = stopwatchMatch.time;
            YandexGame.NewLeaderboardScores("BetterTime", Mathf.RoundToInt(stopwatchMatch.time) * 1000);
            YandexGame.SaveProgress();
        }
        stopwatchMatch.Stop();
    }

    public void SetStatisticTexts(MatchStatisticController controller)
    {
        heroData.SetStatisticText(controller);
        currentGun.SetStatisticText(controller);
    }
}
