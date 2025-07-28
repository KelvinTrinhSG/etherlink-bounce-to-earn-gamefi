using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBorderSpawnSettings : ObjectSpawnSettings
{
    public ColorsPreset preferedPreset;

    public LevelBorderSpawnSettings(ColorsPreset preferedPreset, Vector3 additionaPosition) : base(ObjectType.LevelBorder, additionaPosition)
    {
        this.preferedPreset = preferedPreset;
    }
}