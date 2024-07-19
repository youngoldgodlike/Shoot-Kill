using Assets.Prefabs.Characters.MainHero.Scripts;
using Assets.UI.Prefabs.Abilities.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

namespace _Shoot_Kill.UI.Prefabs.Menu.PauseMenu.Scripts
{
    public class PauseMenu : Menu<PauseMenu>
    {
        [SerializeField] private AbilityWindowManager _abilityWindowManager;
        
        [SerializeField] private GameObject _pauseWindow;
        [SerializeField] private GameObject _settingsWindow;
        [SerializeField] private GameObject _statisticWindow;
        public bool isOpen { get; private set; }
        private GameSession _gameSession;

        [Inject]
        private void Initialize(GameSession gameSession) =>
            _gameSession = gameSession;
        
        private void Update()
        {
            if (HeroHealth.instance.isDie || _abilityWindowManager.isOpenAbilityWindow) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isOpen)
                {

                    _pauseWindow.SetActive(true);
                    Time.timeScale = 0f;
                    _gameSession.UIIsActive.Value = isOpen = true;
                }
                else
                    CloseMenu();
            }
        }

        public void CloseMenu()
        {
            animator.SetTrigger(IsClose);
            onClose = () =>
            {
                Time.timeScale = 1f;
                _gameSession.UIIsActive.Value = isOpen = false;
                _pauseWindow.SetActive(false);
            };
        }

        public void OpenSettings()
        {
            _pauseWindow.SetActive(false);
            _settingsWindow.SetActive(true);
        }

        public void BackPauseMenu()
        {
            _statisticWindow.SetActive(false);
            _settingsWindow.SetActive(false);
            _pauseWindow.SetActive(true);
        }

        public void OpenStatistic()
        {
            _pauseWindow.SetActive(false);
            _statisticWindow.SetActive(true);
        }

        public void BackMainMenu()
        {
            animator.SetTrigger(IsClose);
            onClose = () =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            };
        }
        
    }
}