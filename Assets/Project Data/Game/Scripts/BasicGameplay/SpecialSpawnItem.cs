using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialSpawnItem
{
    public Type type;

    public enum Type
    {
        None,
        Object,
        Mob,
        Moving,
    }

    public SpecialSpawnItem()
    {
        type = Type.None;
    }

    public SpecialSpawnItem(Type itemType)
    {
        type = itemType;
    }
}