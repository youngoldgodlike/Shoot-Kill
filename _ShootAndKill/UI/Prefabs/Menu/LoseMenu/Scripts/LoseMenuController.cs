using Assets.Prefabs.Characters.MainHero.Scripts;
using Assets.UI.Architecture.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;
using Zenject;

public class LoseMenuController : Menu<LoseMenuController>
{
    [SerializeField] private GameObject _loseMenu;
    [SerializeField] private TimeText _timeText;
    private GameSession _gameSession;

    [Inject]
    private void Initialize(GameSession gameSession) =>
        _gameSession = gameSession;
    
    protected void Start()
    {
        HeroHealth.instance.onDie.AddListener(ShowMenu); 
    }

    private void ShowMenu()
    {
        Time.timeScale = 0f;
        
        _gameSession.UIIsActive.Value = true;
        _loseMenu.SetActive(true);
        _gameSession.matchData.CheckBetterTime(_timeText.SetTimeText);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        _gameSession.UIIsActive.Value = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void BackMainMenu()
    {
        _gameSession.UIIsActive.Value = false; 
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
