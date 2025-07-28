using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreSkinDefault : StoreProduct
{
    public StoreSkinDefault()
    {
        productBehaviourType = BehaviourType.Default;
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
        return true;
    }

    public override float Progress()
    {
        return 1;
    }
}
