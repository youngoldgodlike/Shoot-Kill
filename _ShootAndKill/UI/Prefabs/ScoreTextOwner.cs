using Assets.UI.Architecture.Scripts;
using YG;

public class ScoreTextOwner : TimeText
{
    private void Awake() =>
        SetTimeText(YandexGame.savesData.betterMatchTime);
    
}
