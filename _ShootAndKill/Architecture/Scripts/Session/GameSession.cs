using _Shoot_Kill.Architecture.Scripts.Session;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[RequireComponent(typeof(MatchData), typeof(SettingsData))]
public class GameSession : MonoBehaviour
{
    public MatchData matchData { get; private set; }
    public SettingsData settingsData { get; private set; }
    public ReactiveProperty<bool> UIIsActive { get; set; }
    
    [Inject]
    public void Initialize()
    {
        UIIsActive = new ReactiveProperty<bool>();

        matchData = GetComponent<MatchData>();
        settingsData = GetComponent<SettingsData>();
        
        SceneManager.sceneLoaded += OnChangedScene;
    }

    private void Awake()
    {
        if (IsSessionExit())
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this);
    }

    private bool IsSessionExit()
    {
        var sessions = FindObjectsOfType<GameSession>();
        foreach (var gameSession in sessions)
        {
            if (gameSession != this)
                return true;
        }

        return false;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnChangedScene;
    }

    private void OnChangedScene(Scene scene, LoadSceneMode sceneMode)
    {
        matchData.OnChangedScene(scene);
    }
}
