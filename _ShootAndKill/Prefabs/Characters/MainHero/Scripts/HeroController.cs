using _Shoot_Kill.Architecture.Scripts;
using Assets.Prefabs.Guns.Scripts;
using Assets.UI.Architecture.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Prefabs.Characters.MainHero.Scripts
{
    public class HeroController : Singleton<HeroController>
    {
        [SerializeField] private Animator _heroAnimator;
        [SerializeField] private ProjectilePool _pool;
        [SerializeField] private Transform _handle;
        [SerializeField] private ProgressBarWithText _storeProgressBar;
        [SerializeField] private Image _reloadAbilityBar;

        public ProjectileAttack currentGun { get; private set; }
        private WeaponData weaponData => _gameSession.matchData.currentWeaponData;
        private MatchData matchData => _gameSession.matchData;

        private GameSession _gameSession;

        [Inject]
        private void Initialize(GameSession gameSession)
        {
            _gameSession = gameSession;
            Initialize();
        }

        private void Start()
        {
             matchData.StopStopwatch();
             matchData.StartMatchStopwatch();
        }

        private void Initialize()
        {
            _heroAnimator.runtimeAnimatorController = weaponData.animator;
            
            var gun = Instantiate(weaponData.gun, _handle);
            gun.Init(_pool, _storeProgressBar, _reloadAbilityBar);
            
            gun.transform.position = _handle.position;
            gun.transform.rotation = transform.rotation;
            
            currentGun = gun;
            
            matchData.SetProjectileAttack(currentGun);
        }
    }
}
