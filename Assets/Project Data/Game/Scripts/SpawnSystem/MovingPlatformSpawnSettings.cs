using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovingPlatformSpawnSettings : SpecialSpawnItem
{
    public bool inited = false;

    public int platformsAmount;
    public int movingSpaceLength;

    public MovingPlatformSpawnSettings(int platformsAmount, int movingSpaceLength) : base(Type.Moving)
    {
        inited = false;
        this.platformsAmount = platformsAmount;
        this.movingSpaceLength = movingSpaceLength;
    }
}
