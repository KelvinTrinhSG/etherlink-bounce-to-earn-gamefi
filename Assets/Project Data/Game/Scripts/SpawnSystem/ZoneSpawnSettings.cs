#pragma warning disable 0649

using UnityEngine;

[System.Serializable]
public class ZoneSpawnSettings
{
    public Zone type;
    public int zoneSpawnPercentage;

    [Space(5)]
    public int platformsAmount;

    [Header("Platforms Spawn Settigns")]
    [SerializeField]
    private PlatformsSettingsContainer platformsSettings;

    public QueueElement[] GetRandomSpawnQueue(float platformHeight)
    {
        return platformsSettings.GetRandomPlatformSpawnQueue(platformHeight);
    }
}
