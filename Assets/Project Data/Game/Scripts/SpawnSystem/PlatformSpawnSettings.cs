using UnityEngine;

[System.Serializable]
public class PlatformSpawnSettings
{
    // main type of platform
    public PlatformType basicPlatformType;

    // chance for platform appearing
    public int platformSpawnPercentage;

    // number of 1 unit platforms of basic type in row
    public int minPlatformsAmount = 1;
    public int maxPlatformsAmount = 1;

    // random amount of normal platforms around basic platform (if not needed just left 0)
    // when used with moving platform  - it's min and max moving distance (editor should display other name)
    public int minExtraPlatformsAmount = 0;
    public int maxExtraPlatformsAmount = 0; // update | if used on moving: add in editor check - max moving distance should be <= public static int MaxAllowedPlatformsOffset


    public int GetRandomBasicPlatformsAmount()
    {
        return Random.Range(minPlatformsAmount, maxPlatformsAmount + 1);
    }

    public int GetRandomExtraPlatformsAmount()
    {
        return Random.Range(minExtraPlatformsAmount, maxExtraPlatformsAmount + 1);
    }
}
