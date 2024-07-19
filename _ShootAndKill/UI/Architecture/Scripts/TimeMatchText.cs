using Assets.UI.Architecture.Scripts;
using Zenject;

public class TimeMatchText : TimeText
{
    private GameSession _gameSession;

    [Inject]
    private void Initialize(GameSession gameSession)
    {
        _gameSession = gameSession;
    }
    
    private void Awake()
    {
        _gameSession.matchData.stopwatchMatch.onTick += SetTimeText;
    }
    private void OnDisable()
    {
        _gameSession.matchData.stopwatchMatch.onTick -= SetTimeText;
    }
}
