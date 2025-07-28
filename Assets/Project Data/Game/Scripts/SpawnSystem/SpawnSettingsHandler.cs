using System.Collections.Generic;
using UnityEngine;
using Watermelon;

[CreateAssetMenu(fileName = "SpawnSettingsHandler", menuName = "SpawnSystem/SpawnSettingsHandler")]
public class SpawnSettingsHandler : ScriptableObject
{
    private static SpawnSettingsHandler instance;
    [DisableField]
    public static int maxAllowedPlatformsOffset = 7;
    [DisableField]
    public static int minDistToNextComboPlatform = 4;
    [DisableField]
    public static int maxDistToNextComboPlatform = 7;

    [Header("Main settings")]
    public int spawnStartPointX = -8;
    [Range(0f, 1f)]
    public float maxPlatformsHeightOffset;
    [Range(0f, 1f)]
    public float powerUpsSpawnChance;
    public int minPlatformsBeforeNextPowerUp;

    public bool powerupsSpawnChanceOverride = false;
    public List<PowerupSpawnChance> powerupsSpawnChances = new List<PowerupSpawnChance>();

    [Header("Background settings")]
    public int backgroundDepth = 3; // number of background platforms spawned at one line
    public int backgroundOffset = 2;
    public float minBgHeight = -1f;
    public float maxBgHeight = 2f;
    [Range(0f, 1f)]
    public float landSmooth = 0f;


    [Header("Platforms spawn settings")]
    public List<LevelsHolder> levels;

    // privates ////////////////////////////////////////////////////////////////

    #region Queue Presets

    private static QueueElement[] revivePlatformQueue = new QueueElement[]
    {
        QueueElement.Air,
        QueueElement.Air,
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        QueueElement.Air,
        QueueElement.Air,
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
    };

    private static QueueElement[] chalangeFinishQueue = new QueueElement[]
   {
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
        new QueueElement(PlatformType.Normal,0,null),
   };

    //public static QueueElement[] presetSetupLayout = new QueueElement[]
    //{
    //    QueueElement.Air,
    //    QueueElement.Air,
    //    new QueueElement(PlatformType.Normal,0,null),
    //    new QueueElement(PlatformType.Normal,0,null),
    //    new QueueElement(PlatformType.Normal,0,null),
    //    new QueueElement(PlatformType.Normal,0,null),
    //    QueueElement.Air,
    //    QueueElement.Air,
    //    QueueElement.Air,
    //    new QueueElement(PlatformType.Jump,0,null),
    //    new QueueElement(PlatformType.Jump,0,null),
    //    new QueueElement(PlatformType.Jump,0,null),
    //    QueueElement.Air,
    //    QueueElement.Air,
    //    new QueueElement(PlatformType.Spiked,0,null),
    //    new QueueElement(PlatformType.Spiked,0,null),
    //    new QueueElement(PlatformType.Spiked,0,null),
    //    new QueueElement(PlatformType.Spiked,0,null),
    //    QueueElement.Air,
    //    QueueElement.Air,
    //    new QueueElement(PlatformType.Mob,0,null),
    //    new QueueElement(PlatformType.Mob,0,null),
    //    new QueueElement(PlatformType.Mob,0,null),
    //    new QueueElement(PlatformType.Mob,0,null),
    //    QueueElement.Air,
    //    QueueElement.Air,
    //    new QueueElement(PlatformType.Moving,0,null),
    //    new QueueElement(PlatformType.Moving,0,null),
    //    QueueElement.Air,
    //    QueueElement.Air,
    //};
    //public static QueueElement[] tutorialQueue = new QueueElement[]
    //{
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Jump, 0, null),
    //        new QueueElement(PlatformType.Jump, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        new QueueElement(PlatformType.Normal, 0, null),
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //        QueueElement.Air,
    //};

    #endregion

    //private SpawnSettings currentDistanceSpawnSettings;
    private ZoneSpawnSettings currentZoneSettings;
    public static Location lastSpawnedLocation = Location.None;
    private Zone currentZone = Zone.None;

    private bool isMobPlatformAllowed;

    private int currentSettingsIndex;
    private int additionalSpaceBeforePlatform; // extra air space created by moving platform in most left position

    private int zonePlatformsPassed;
    private int inifinitModePlatformsSpawned;
    private int platformsSpawnedAfterLastPowerUp;

    private float previousPlatformAdditionalHeight = 0;

    // new variables
    private LevelPreset currentLevel;

    private bool onLevelChanged;

    private int[] currentLevelLocationsAppearingIndexes;

    private int currentLevelHolderIndex;
    private int currentLevelIndex;
    private int currentLevelPlatformsSpawned;
    private int currentLevelNumber;
    private int locationsSpawned;

    public static int AdditionalSpaceBeforePlatform
    {
        get { return instance.additionalSpaceBeforePlatform; }
        set { instance.additionalSpaceBeforePlatform = value; }
    }

    public static bool IsMobPlatformAllowed
    {
        get { return instance.isMobPlatformAllowed; }
        set { instance.isMobPlatformAllowed = true; }
    }

    public static bool SpawnPowerUp
    {
        get
        {
            instance.platformsSpawnedAfterLastPowerUp++;
            if (instance.platformsSpawnedAfterLastPowerUp > instance.minPlatformsBeforeNextPowerUp)
            {
                bool result = Random.Range(0f, 1f) < instance.powerUpsSpawnChance;
                if (result)
                {
                    instance.platformsSpawnedAfterLastPowerUp = 0;
                }

                return result;
            }

            return false;
        }
    }

    public static Zone CurrentZone
    {
        get { return instance.currentZone; }
    }

    public static QueueElement[] ReviveSpawnQueue
    {
        get { return revivePlatformQueue; }
    }
    
    public static QueueElement[] ChalangeFinishSpawnQueue
    {
        get { return chalangeFinishQueue; }
    }

    public static int MaxAllowedPlatformsOffset
    {
        get { return (int)(maxAllowedPlatformsOffset * (1 + (instance.currentLevel.speedCoef - 1f) * 0.7f)); }
    }

    public void Init()
    {
        instance = this;
        // new system
        currentLevelNumber = 1;
        currentLevelHolderIndex = 0;
        levels[0].levelPresets.Shuffle();
        currentLevelIndex = 0;
        currentLevel = levels[0].levelPresets[0];
        currentLevelPlatformsSpawned = 0;

        InitializeLevelLocationSpawnData();

        additionalSpaceBeforePlatform = 0;
        platformsSpawnedAfterLastPowerUp = 0;
        lastSpawnedLocation = Location.None;
        currentZone = Zone.None;
    }


    public QueueElement[] GetNextSpawnQueue()
    {
        LevelPreset level = GetCurrentLevel();
        currentLevelPlatformsSpawned++;

        QueueElement[] queue = new QueueElement[0];

        // trying get location queue
        if (currentLevelLocationsAppearingIndexes.Length > 0 && locationsSpawned < currentLevelLocationsAppearingIndexes.Length && currentLevelPlatformsSpawned == currentLevelLocationsAppearingIndexes[locationsSpawned])
        {
            if (currentLevelLocationsAppearingIndexes[locationsSpawned] != -1)
            {
                queue = currentLevel.GetLocationQueue(currentLevel.locationsSpawnSettings[locationsSpawned], GetRandomPlatformHeightOffset());
            }
            else
            {
                queue = level.GetRandomSpawnQueue(GetRandomPlatformHeightOffset());
            }

            locationsSpawned++;
        }
        // getting platform spawn queue
        else
        {
            queue = level.GetRandomSpawnQueue(GetRandomPlatformHeightOffset());
        }

        if (onLevelChanged)
        {
            onLevelChanged = false;

            QueueElement element = queue[0];
            LevelBorderSpawnSettings sett = new LevelBorderSpawnSettings(currentLevel.preferedPreset, Vector3.zero);
            element.specialSpawnItem = sett;
            queue[0] = element;
        }

        return queue;
    }

    private LevelPreset GetCurrentLevel()
    {
        // if current level is active
        if (currentLevelPlatformsSpawned < currentLevel.platformsAmount)
        {
            return currentLevel;
        }
        else
        {
            currentLevelNumber++;
            onLevelChanged = true;

            // if we have not reached max level of current holder
            if (currentLevelNumber <= levels[currentLevelHolderIndex].maxLevel)
            {
                //taking next level
                currentLevelIndex++;
                if (currentLevelIndex >= levels[currentLevelHolderIndex].levelPresets.Count)
                {
                    currentLevelIndex = 0;
                }

                currentLevel = levels[currentLevelHolderIndex].levelPresets[currentLevelIndex];
                InitializeLevelLocationSpawnData();
            }
            else
            {
                // if there is next holders
                if (currentLevelHolderIndex + 1 < levels.Count)
                {
                    // initializing next holder and returning first level
                    currentLevelHolderIndex++;
                    currentLevelIndex = 0;

                    levels[currentLevelHolderIndex].levelPresets.Shuffle();
                    currentLevel = levels[currentLevelHolderIndex].levelPresets[currentLevelIndex];
                    InitializeLevelLocationSpawnData();
                }
                // if there's no next holders - infinitely returning levels from current
                else
                {
                    currentLevelIndex++;
                    if (currentLevelIndex >= levels[currentLevelHolderIndex].levelPresets.Count)
                    {
                        currentLevelIndex = 0;
                    }

                    currentLevel = levels[currentLevelHolderIndex].levelPresets[currentLevelIndex];
                    InitializeLevelLocationSpawnData();
                }
            }

            currentLevelPlatformsSpawned = 0;
            return currentLevel;
        }
    }


    private float GetRandomPlatformHeightOffset()
    {
        float value = Random.Range(-maxPlatformsHeightOffset, maxPlatformsHeightOffset);

        while (value == previousPlatformAdditionalHeight)
        {
            value = Random.Range(-maxPlatformsHeightOffset, maxPlatformsHeightOffset);
        }

        previousPlatformAdditionalHeight = value;
        return value;
    }

    public static PowerUpType GetPowerupType()
    {
        if (instance.powerupsSpawnChanceOverride)
        {
            int randomPercentage = Random.Range(1, 101);
            int valueToCheck = 0;

            for (int i = 0; i < instance.powerupsSpawnChances.Count; i++)
            {
                valueToCheck += instance.powerupsSpawnChances[i].spawnChance;

                if (randomPercentage <= valueToCheck)
                {
                    return instance.powerupsSpawnChances[i].type;
                }
            }

            Debug.LogError("Random percentage is out of bounds. Fix powerups spawn percentages. Outdone percentage : " + randomPercentage + ".");
            return instance.powerupsSpawnChances[0].type;
        }
        else
        {
            return (PowerUpType)Random.Range(0, PowerUp.powerUpsCount);
        }
    }

    public int GetNextComboSpacing()
    {
        return Mathf.FloorToInt(Random.Range(minDistToNextComboPlatform, MaxAllowedPlatformsOffset));
    }

    private void InitializeLevelLocationSpawnData()
    {
        locationsSpawned = 0;

        currentLevelLocationsAppearingIndexes = new int[currentLevel.locationsSpawnSettings.Count];

        if (currentLevel.locationsSpawnSettings.Count == 0 || currentLevel.locationsSpawnChance == 0)
            return;

        int step = currentLevel.platformsAmount / (currentLevel.locationsSpawnSettings.Count);
        step = (int)(step * 0.9f);


        for (int i = 0; i < currentLevel.locationsSpawnSettings.Count; i++)
        {
            if (Random.Range(0f, 1f) < currentLevel.locationsSpawnChance)
            {
                currentLevelLocationsAppearingIndexes[i] = Random.Range(step * i + 1, step * (i + 1));
            }
            else
            {
                currentLevelLocationsAppearingIndexes[i] = -1;
            }
        }
    }

    [System.Serializable]
    public class PowerupSpawnChance
    {
        public PowerUpType type;
        public int spawnChance;
    }
}