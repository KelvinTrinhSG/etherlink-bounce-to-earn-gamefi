using UnityEngine;
using Watermelon;

public class StoreSkinShareProduct : StoreProduct
{
    [SerializeField, MultilanguageWord("share"), Tooltip("{0} - score, {1} - link")]
    private string shareKey;

    
    public StoreSkinShareProduct()
    {
        productBehaviourType = BehaviourType.Share;
    }

    public override void Init()
    {

    }

    public override void Buy()
    {
        //Do nothing
    }

    public override bool IsOpened()
    {
        return PrefsSettings.GetBool(PrefsSettings.Key.IsShareSkinUnlocked);
    }

    public override bool Check()
    {
        return true;
    }

    public override float Progress()
    {
        if (IsOpened())
            return 1;

        return 0;
    }

    public void Share()
    {
        ShareController.ShareMessage();

        //PrefsSettings.SetBool(PrefsSettings.Key.IsShareSkinUnlocked, true);
    }
}
