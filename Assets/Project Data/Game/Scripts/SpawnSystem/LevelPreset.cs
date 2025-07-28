using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

[CreateAssetMenu(fileName = "LevelPreset", menuName = "SpawnSystem/Level spawn preset")]
public class LevelPreset : ScriptableObject
{
    public int platformsAmount;

    [Range(1f, 2f)]
    public float speedCoef = 1f;

    [Header("Platforms spawn Settigns")]
    public PlatformsSettingsContainer platformsSpawnSettings;

    [Header("Locations spawn settings")]
    [Space(5), Range(0f, 1f)]
    public float locationsSpawnChance;

    [Space(5)]
    public List<Location> locationsSpawnSettings = new List<Location>();

    [Space(10)]
    public ColorsPreset preferedPreset;

    public static List<QueueElement[]> locationsSpawnQueues = new List<QueueElement[]>()
    {
        new QueueElement[] // Jump Canyon
        {
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal, 0, null),
            new QueueElement(PlatformType.Normal, 0, new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.WarningSign, Vector3.zero.SetZ(0.5f))),
            new QueueElement(PlatformType.Normal, 0, null),
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Jump, 0, null),
            new QueueElement(PlatformType.Jump, 0, null),
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.CanyonPassedTrigger, Vector3.zero)),
            new QueueElement(PlatformType.Normal,0,null),
            QueueElement.Air,
        },
        new QueueElement[] // Energy Field Canyon
        {
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.WarningSign, Vector3.zero.SetZ(0.5f))),
            new QueueElement(PlatformType.Normal,0,null),
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal,0, null),
            new QueueElement(PlatformType.Normal,0,new PowerUpSpawnSettings(PowerUpType.EnergyField, Vector3.zero.SetY(2f))),
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.CanyonPassedTrigger, Vector3.zero)),
            new QueueElement(PlatformType.Normal,0,null),
            QueueElement.Air,
        },
        new QueueElement[] // Portal Canyon
        {
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.WarningSign, Vector3.zero.SetZ(0.5f))),
            new QueueElement(PlatformType.Normal,0,null),
            QueueElement.Air,
            QueueElement.Air,new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.PortalIn, Vector3.zero.SetY(0.5f))),
            new QueueElement(PlatformType.Normal,0,null),
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0, new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.PortalOut, new Vector3(0f, 4f)), true),
            new QueueElement(PlatformType.Normal,0,null),
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,null),
            new QueueElement(PlatformType.Normal,0,null),
            QueueElement.Air,
        }
    };

    public QueueElement[] GetRandomSpawnQueue(float platformHeight)
    {
        return platformsSpawnSettings.GetRandomPlatformSpawnQueue(platformHeight);
    }

    public QueueElement[] GetLocationQueue(Location location, float locationHeight)
    {
        if (location == Location.JumpCanyon)
        {
            return GetLocationSpawnQueue(0, locationHeight);
        }
        else if (location == Location.EnergyFieldCanyon)
        {
            return GetLocationSpawnQueue(1, locationHeight);
        }
        else
        {
            return GetLocationSpawnQueue(2, locationHeight);
        }
    }

    private QueueElement[] GetLocationSpawnQueue(int locationID, float platformHeight)
    {
        QueueElement[] origin = locationsSpawnQueues[locationID];
        QueueElement[] queue = new QueueElement[origin.Length];

        for (int i = 0; i < queue.Length; i++)
        {
            queue[i] = new QueueElement((PlatformType)origin[i].platformID, origin[i].platformHeight + platformHeight, origin[i].specialSpawnItem);
        }

        return queue;
    }
}