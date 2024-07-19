using Cysharp.Threading.Tasks;
using Helpers.Debugging;
using SpawnSystem;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace GameBootstraps
{
    public class GameplayBootstrapper : MonoBehaviour
    {
        [SerializeField] private WaveCreator _waveCreator;
        [SerializeField] private SpawnController _spawnController;
        //[SerializeField] private GameLogger _log;

        private GameSession _gameSession;
        
        [Inject]
        private void Initialize(GameSession gameSession)
        {
            _gameSession = gameSession;
        }
        private async UniTaskVoid Awake() {
            if(!CheckDependencies()) return;

           // _log.Log("Инициализация необходимых компонентов...");

            await UniTask.WaitUntil(() => !_gameSession.matchData.IsUnityNull());
            
            var hero = _gameSession.matchData.heroData;
            var gun = _gameSession.matchData.currentGun;

            await UniTask.WaitUntil(() =>
                !hero.IsUnityNull() &&
                !gun.IsUnityNull());
            _waveCreator.Initialize(hero, gun);
            
            //_spawnController.Activate();
            
           // _log.Log("Успешно");
        }

        private bool CheckDependencies() {
            var creator = _waveCreator.IsUnityNull();
            var controller = _spawnController.IsUnityNull();

            if (creator || controller) {
              //  _log.ErrorLog("Зависимостей не хватает!", this);
              //  _log.Log($"NULL CHECKING. WaveCreator: {creator}, SpawnController: {controller}", this);
                
                return false;
            }
            
            //_log.Log("Все зависимости на месте");
            return true;
        }
    }
}
