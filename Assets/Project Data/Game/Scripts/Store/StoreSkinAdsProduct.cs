using UnityEngine;
using Watermelon;

public class StoreSkinAdsProduct : StoreProduct
{
    public int requiredViews = 3;

    public StoreSkinAdsProduct()
    {
        productBehaviourType = BehaviourType.Ads;
    }

    public override void Init()
    {

    }

    public override void Buy()
    {
        //Do nothing
    }

    public override bool Check()
    {
        return true;
    }

    public override bool IsOpened()
    {
        return PrefsSettings.GetInt(PrefsSettings.Key.AdsSkinUnlockProgress) == 0;
    }

    public override float Progress()
    {
        if (IsOpened())
            return 1;

        return (float)(requiredViews - GetCurrentViews()) / requiredViews;
    }

    public int GetCurrentViews()
    {
        return PrefsSettings.GetInt(PrefsSettings.Key.AdsSkinUnlockProgress);
    }

    public void ShowAds(System.Action<int> callback)
    {
        AdsManager.ShowRewardBasedVideo(AdsManager.Settings.RewardedVideoType, (hasReward) =>
        {
            if (hasReward)
            {
                int requiredViews = PrefsSettings.GetInt(PrefsSettings.Key.AdsSkinUnlockProgress) - 1;

                PrefsSettings.SetInt(PrefsSettings.Key.AdsSkinUnlockProgress, requiredViews);

                if (callback != null)
                    callback.Invoke(requiredViews);
            }
        });
    }
}
