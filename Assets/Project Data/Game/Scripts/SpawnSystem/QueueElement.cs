using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct QueueElement
{
    public int platformID;
    public float platformHeight;
    public bool isComboPlatform;
    public SpecialSpawnItem specialSpawnItem;

    public QueueElement(PlatformType platformType, float heigth, SpecialSpawnItem specialSpawnItem, bool isComboPlatform = false)
    {
        platformID = (int)platformType;
        platformHeight = heigth;
        this.specialSpawnItem = specialSpawnItem;

        this.isComboPlatform = platformType == PlatformType.Jump || isComboPlatform;
    }

    public static QueueElement Air
    {
        get { return new QueueElement(PlatformType.Shield, 0f, null); }
    }

    public override bool Equals(object obj)
    {
        return obj is QueueElement && this == (QueueElement)obj;
    }

    public override int GetHashCode()
    {
        return platformID.GetHashCode() ^ platformHeight.GetHashCode() ^ specialSpawnItem.GetHashCode();
    }

    public static bool operator ==(QueueElement x, QueueElement y)
    {
        return x.platformID == y.platformID && x.platformHeight == y.platformHeight && x.specialSpawnItem == y.specialSpawnItem;
    }
    public static bool operator !=(QueueElement x, QueueElement y)
    {
        return !(x == y);
    }
}