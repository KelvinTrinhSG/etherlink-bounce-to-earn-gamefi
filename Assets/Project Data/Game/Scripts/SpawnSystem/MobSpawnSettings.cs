using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MobSpawnSettings : SpecialSpawnItem
{
    public int groundPlatformsAmount;

    public MobSpawnSettings(int groundPlatformsAmount) : base (Type.Mob)
    {
        this.groundPlatformsAmount = groundPlatformsAmount;
    }
}