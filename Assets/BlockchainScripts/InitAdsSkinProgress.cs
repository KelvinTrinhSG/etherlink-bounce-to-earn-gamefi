using UnityEngine;
using Watermelon;

public class InitAdsSkinProgress : MonoBehaviour
{
    [Header("Số lượt xem cần thiết để mở khóa skin")]
    public int requiredViews = 0;

    private void Start()
    {
        if (PlayerDataManager.Instance.vipNFT > 0)
        {
            PrefsSettings.SetInt(PrefsSettings.Key.AdsSkinUnlockProgress, requiredViews);
            PrefsSettings.SetBool(PrefsSettings.Key.IsShareSkinUnlocked, true);
            Debug.Log($"✅ VIP detected. AdsSkinUnlockProgress set to {requiredViews}");
        }
        else
        {
            Debug.Log("⚠️ No VIP NFT. Skipping AdsSkinUnlockProgress setup.");
        }
    }

}
