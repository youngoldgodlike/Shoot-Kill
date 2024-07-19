using YG;

public static class YGAdHandler 
{

    public static void ShowAd()
    {
        YandexGame.FullscreenShow();
    }

    public static void ShowRewardedAd(int id)
    {
        YandexGame.RewVideoShow(id);
    }

    
}
