using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using Random = UnityEngine.Random;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance;

    public SpawnSettingsHandler spawnSetings;

    [Header("References")]
    public Transform rightSpawnOriginTransform;
    public Transform bottomSpawnOriginTransform;
    public Transform disablerTransform;

    private SpawnSettingsHandler currentSpawnSettings;
    private List<QueueElement> spawnQueue = new List<QueueElement>();
    private List<byte[]> relaxZoneSpawnQueue = new List<byte[]>();
    private Pool[] platformsPools;
    private Pool[] powerUpsPools;
    private Pool backgroundPlatformsPool;
    private Pool platformMobsPool;
    private Pool warningSignPool;
    private Pool portalInPool;
    private Pool portalOutPool;
    private Pool powerUpDisablerPool;
    private Pool canyonPassedTriggerPool;
    private Pool flyingCliffPool;
    private Pool levelBorderPool;
    private GameObject lastSpawnedColumn;
    private Platform currentPlatformStart;
    private Portal lastSpawnedPortalIn;

    private bool isActive;
    private bool needToGenerateCollider;
    private bool isChalangeMode;
    private bool justSpawnedAir;
    private bool spikedPlatfromIsSpawningNow;

    private float[] currentLineHeights;
    private float nextSpawnHeight;
    private float relaxZoneStartPoint;
    private float prevPlatformHeight;

    private int nextLinePositionX;
    private int chalangeFinishSpawnPoint;
    private int distToNextComboPlatform;

    public bool Active
    {
        get { return isActive; }
        set { isActive = value; }
    }

    private void Awake()
    {
        instance = this;

        // initializing platforms pools
        Array typeValues = Enum.GetValues(typeof(PlatformType));
        platformsPools = new Pool[typeValues.Length];

        foreach (int value in typeValues)
        {
            platformsPools[value] = PoolManager.GetPoolByName(((PlatformType)value) + "Platform");
        }

        // initializing powerUps pools
        typeValues = Enum.GetValues(typeof(PowerUpType));
        powerUpsPools = new Pool[typeValues.Length];

        foreach (int value in typeValues)
        {
            powerUpsPools[value] = PoolManager.GetPoolByName(((PowerUpType)value) + "PowerUp");
        }

        backgroundPlatformsPool = PoolManager.GetPoolByName("BackgroundPlatform");
        platformMobsPool = PoolManager.GetPoolByName("PlatformMob");
        warningSignPool = PoolManager.GetPoolByName("WarningSign");
        portalInPool = PoolManager.GetPoolByName("PortalIn");
        portalOutPool = PoolManager.GetPoolByName("PortalOut");
        powerUpDisablerPool = PoolManager.GetPoolByName("PowerUpDisabler");
        canyonPassedTriggerPool = PoolManager.GetPoolByName("CanyonPassedTrigger");
        flyingCliffPool = PoolManager.GetPoolByName("FlyingCliff");
        levelBorderPool = PoolManager.GetPoolByName("LevelBorder");
    }
    
    public void LaunchOnInfiniteMode()
    {
        isChalangeMode = false;
        LaunchSpawnController(spawnSetings);
    }

    public void LaunchOnChalangeMode(SpawnSettingsHandler spawnSettings, int finishPosition)
    {
        isChalangeMode = true;
        chalangeFinishSpawnPoint = finishPosition;
        LaunchSpawnController(spawnSettings);
    }

    private void LaunchSpawnController(SpawnSettingsHandler spawnSettings)
    {
        isActive = true;

        currentSpawnSettings = spawnSettings;
        currentSpawnSettings.Init();

        StartGeneration();
    }

    private void StartGeneration()
    {
        nextLinePositionX = currentSpawnSettings.spawnStartPointX;
        currentLineHeights = new float[currentSpawnSettings.backgroundDepth];
        needToGenerateCollider = true;
                
        GenerateStartPlatforms();
        StartCoroutine(GenerationCoroutine());
    }
    
    private IEnumerator GenerationCoroutine()
    {
        while (isActive)
        {
            if (isChalangeMode && nextLinePositionX >= chalangeFinishSpawnPoint)
            {
                spawnQueue.Clear();
                spawnQueue.AddRange(SpawnSettingsHandler.ChalangeFinishSpawnQueue);
            }

            yield return WaitUntillNextLine();

            GenerateLine();
            SpawnLine();
        }
    }
    
    private IEnumerator WaitUntillNextLine()
    {
        while (rightSpawnOriginTransform.position.x + 1 < nextLinePositionX && isActive)
        {
            yield return null;
        }
    }

    private void GenerateStartPlatforms()
    {
        spawnQueue = new List<QueueElement>()
        {
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal, 0, null),
            new QueueElement(PlatformType.Normal, 0, null),
            new QueueElement(PlatformType.Normal, 0, null),
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal, 0, null),
            new QueueElement(PlatformType.Normal, 0, null),
            new QueueElement(PlatformType.Normal, 0, null),
            QueueElement.Air,
            QueueElement.Air,
            new QueueElement(PlatformType.Normal, 0, null),
            new QueueElement(PlatformType.Normal, 0, null),
            new QueueElement(PlatformType.Normal, 0, null),
        };

        distToNextComboPlatform = spawnQueue.Count;

        while (rightSpawnOriginTransform.position.x + 1 > nextLinePositionX && isActive)
        {
            GenerateLine();
            SpawnLine();
        }
    }

    private void GenerateLine()
    {
        if (spawnQueue.Count <= 15)
        {
            QueueElement[] platformSpawnQueue = currentSpawnSettings.GetNextSpawnQueue();
            spawnQueue.AddRange(platformSpawnQueue);
        }
    }


    private void SpawnLine()
    {
        QueueElement currentElement = spawnQueue[0];

        // positive id means that we need to spawn platform
        if (currentElement.platformID >= 0 && currentElement.platformID != 5)
        {
            lastSpawnedColumn = platformsPools[currentElement.platformID].GetPooledObject(new Vector3(nextLinePositionX, currentElement.platformHeight));

            // different platform's heights means that it is different platforms (even if stucked together) - so we need to spawn separate colliders
            if (prevPlatformHeight != currentElement.platformHeight || justSpawnedAir)
            {
                needToGenerateCollider = true;

                currentPlatformStart = lastSpawnedColumn.GetComponent<Platform>();
            }

            // if we just spawned spiked platform we need to generate collider for it personaly | spiked platform's id is 2 (for optimization I do not using cast)
            if (currentElement.platformID == 2)
            {
                needToGenerateCollider = true;
                spikedPlatfromIsSpawningNow = true;
            }

            // adding ref for each platform to it's collidable column
            if (currentPlatformStart != null)
            {
                currentPlatformStart.AddPlatformPart(lastSpawnedColumn, spikedPlatfromIsSpawningNow);
            }

            // spawning special item
            if (currentElement.specialSpawnItem != null)
            {
                SpawnSpecialItem(currentElement);
            }

            // if before we spawned air or spiked platform
            if (needToGenerateCollider)
            {
                prevPlatformHeight = currentElement.platformHeight;

                // if first platform is not spiked - generating big collider
                if (currentElement.platformID != 2)
                {
                    int platformLength = 0;
                    int currentIndex = 0;

                    // while there is elements in queue and it's not air and not spiked (for spiked should be spawned personal collider) and not air/shield which is spawned together with moving platform and platforms heights are equal
                    while (currentIndex < spawnQueue.Count && spawnQueue[currentIndex].platformID >= 0 && spawnQueue[currentIndex].platformID != 2 && spawnQueue[currentIndex].platformID != 5 && spawnQueue[currentIndex].platformHeight == currentElement.platformHeight)
                    {
                        platformLength++;
                        currentIndex++;
                    }

                    BoxCollider2D boxCollider = lastSpawnedColumn.GetComponent<BoxCollider2D>();

                    boxCollider.size = new Vector2(platformLength, 2f);
                    boxCollider.offset = new Vector2(platformLength * 0.5f - 0.5f, -1f);
                    boxCollider.enabled = true;

                    needToGenerateCollider = false;
                }
                // for spiked platform we enabling own collider (to allow detect collisions with spikes)
                else
                {
                    lastSpawnedColumn.GetComponent<BoxCollider2D>().enabled = true;

                    // to ensure that for next platform also will be generated a collider
                    needToGenerateCollider = true;
                }

                justSpawnedAir = false;
            }

            // combo system   
            if (currentElement.isComboPlatform)
            {
                lastSpawnedColumn.GetComponent<Platform>().InitializeCombo();
            }
        }
        // if we need to spawn shield or air
        else if (currentElement.platformID == 5 || currentElement.platformID == -1)
        {
            platformsPools[(int)PlatformType.Shield].GetPooledObject(new Vector3(nextLinePositionX, currentElement.platformHeight));
            needToGenerateCollider = true;
            justSpawnedAir = true;
            spikedPlatfromIsSpawningNow = false;

            if (currentElement.specialSpawnItem != null)
            {
                SpawnSpecialItem(currentElement);
            }
        }

        spawnQueue.RemoveAt(0);

        SpawnBGLine();

        nextLinePositionX += 1;
        distToNextComboPlatform--;
        if (distToNextComboPlatform <= 0)
        {
            InsertNextComboPlatformAtQueue();
        }
    }

    private void InsertNextComboPlatformAtQueue()
    {
        distToNextComboPlatform = currentSpawnSettings.GetNextComboSpacing();
        //Debug.Log("dist: " + distToNextComboPlatform + "   max: " + SpawnSettingsHandler.MaxAllowedPlatformsOffset);
        int indexOfReviewedQueueElement = Mathf.Clamp(distToNextComboPlatform, 0, spawnQueue.Count - 1);

        //green.GetPooledObject(new Vector3(nextLinePositionX + indexOfReviewedQueueElement, 0.5f));
        if (IsAllowedToBeComboForPlatform(spawnQueue[indexOfReviewedQueueElement].platformID))
        {
            SetQueueElementAsCombo(indexOfReviewedQueueElement);
            return;
        }

        int minCheckIndex = SpawnSettingsHandler.minDistToNextComboPlatform;
        int maxCheckIndex = SpawnSettingsHandler.MaxAllowedPlatformsOffset;
        int addition = 0;

        while (true)
        {
            addition++;

            if (indexOfReviewedQueueElement - addition < minCheckIndex && indexOfReviewedQueueElement + addition > maxCheckIndex)
                return;

            if (indexOfReviewedQueueElement + addition <= maxCheckIndex && IsAllowedToBeComboForPlatform(spawnQueue[indexOfReviewedQueueElement + addition].platformID))
            {
                //red.GetPooledObject(new Vector3(nextLinePositionX + indexOfReviewedQueueElement + addition, 0.5f));
                SetQueueElementAsCombo(indexOfReviewedQueueElement + addition);
                distToNextComboPlatform += addition;
                return;
            }
            else if (indexOfReviewedQueueElement + addition <= maxCheckIndex)
            {
                //yellow.GetPooledObject(new Vector3(nextLinePositionX + indexOfReviewedQueueElement + addition, 0.5f));
            }

            if (indexOfReviewedQueueElement - addition >= minCheckIndex && IsAllowedToBeComboForPlatform(spawnQueue[indexOfReviewedQueueElement - addition].platformID))
            {
                //red.GetPooledObject(new Vector3(nextLinePositionX + indexOfReviewedQueueElement - addition, 0.5f));
                SetQueueElementAsCombo(indexOfReviewedQueueElement - addition);
                distToNextComboPlatform -= addition;
                return;
            }
            else if (indexOfReviewedQueueElement - addition >= minCheckIndex)
            {
                //yellow.GetPooledObject(new Vector3(nextLinePositionX + indexOfReviewedQueueElement - addition, 0.5f));
            }
        }
    }

    private void SetQueueElementAsCombo(int indexOfElement)
    {
        QueueElement element = spawnQueue[indexOfElement];
        element.isComboPlatform = true;
        spawnQueue[indexOfElement] = element;
    }

    private bool IsAllowedToBeComboForPlatform(int platformID)
    {
        // allowed for normal || moving || invisible | mob platforms by deffault have 50% chance to be combo
        return platformID == 0 || platformID == 4 || platformID == 6;
    }

    private void SpawnSpecialItem(QueueElement queueElement)
    {
        if (queueElement.specialSpawnItem.type == SpecialSpawnItem.Type.Object)
        {
            ObjectSpawnSettings currentSpawnSettings = (ObjectSpawnSettings)queueElement.specialSpawnItem;

            Vector3 objectPosition = new Vector3(nextLinePositionX, queueElement.platformHeight + (queueElement.platformID == (int)PlatformType.Jump ? 0.2f : 0f)) + currentSpawnSettings.additionalPosition;
            
            if (currentSpawnSettings.objectType == ObjectSpawnSettings.ObjectType.PowerUp && !PlayerController.IsBoosterActive)
            {
                powerUpsPools[(int)((PowerUpSpawnSettings)currentSpawnSettings).powerUpType].GetPooledObject(objectPosition);
            }
            else if (currentSpawnSettings.objectType == ObjectSpawnSettings.ObjectType.LevelBorder)
            {
                levelBorderPool.GetPooledObject(objectPosition).GetComponent<LevelBorderBehaviour>().PreferedColorsPreset = ((LevelBorderSpawnSettings)currentSpawnSettings).preferedPreset;

            }
            else if (currentSpawnSettings.objectType == ObjectSpawnSettings.ObjectType.WarningSign)
            {
                warningSignPool.GetPooledObject(objectPosition);
            }
            else if (currentSpawnSettings.objectType == ObjectSpawnSettings.ObjectType.PortalIn)
            {
                lastSpawnedPortalIn = portalInPool.GetPooledObject(objectPosition).GetComponent<Portal>();
            }
            else if (currentSpawnSettings.objectType == ObjectSpawnSettings.ObjectType.PortalOut)
            {
                lastSpawnedPortalIn.otherSidePortal = portalOutPool.GetPooledObject(objectPosition).GetComponent<Portal>();
            }
            else if (currentSpawnSettings.objectType == ObjectSpawnSettings.ObjectType.PowerUpDisabler)
            {
                powerUpDisablerPool.GetPooledObject(objectPosition);
            }
            else if (currentSpawnSettings.objectType == ObjectSpawnSettings.ObjectType.CanyonPassedTrigger)
            {
                canyonPassedTriggerPool.GetPooledObject(objectPosition);
            }
        }
        else
        {
            if (queueElement.platformID == (int)PlatformType.Mob)
            {
                MobBehaviour mobBehaviour = platformMobsPool.GetPooledObject(new Vector3(nextLinePositionX, queueElement.platformHeight)).GetComponent<MobBehaviour>();
                mobBehaviour.Init(queueElement.specialSpawnItem as MobSpawnSettings, nextLinePositionX);

                if (currentPlatformStart != null)
                {
                    currentPlatformStart.SetMobReference(mobBehaviour);
                }
            }
            else if (queueElement.platformID == (int)PlatformType.Moving)
            {
                lastSpawnedColumn.GetComponent<MovingPlatformBehaviour>().Init(queueElement.specialSpawnItem as MovingPlatformSpawnSettings);
                platformsPools[(int)PlatformType.Shield].GetPooledObject(new Vector3(nextLinePositionX, queueElement.platformHeight));
            }
        }
    }

    private void SpawnBGLine()
    {
        UpdateHeightArray(ref currentLineHeights);

        for (int i = 0; i < currentSpawnSettings.backgroundDepth; i++)
        {
            GameObject platform = backgroundPlatformsPool.GetPooledObject(new Vector3(nextLinePositionX + (i % 2) * 0.1f, currentLineHeights[i] + i * 0.5f, i + 1 + currentSpawnSettings.backgroundOffset));

            if (Random.Range(0f, 1f) < 0.2f)
            {
                platform.GetComponent<BackgroundPlatformData>().StartMovement();
            }
        }
    }

    private void UpdateHeightArray(ref float[] heightList)
    {
        for (int i = 0; i < heightList.Length; i++)
        {
            float newRandomHeight = Random.Range(currentSpawnSettings.minBgHeight, currentSpawnSettings.maxBgHeight);

            heightList[i] = heightList[i] + (newRandomHeight - heightList[i]) * currentSpawnSettings.landSmooth;

            float firstDelta = 0;
            float secondDelta = 0;

            if (i > 0)
            {
                firstDelta = (heightList[i] - heightList[i - 1]) * (1 - currentSpawnSettings.landSmooth);
            }

            if (i < heightList.Length - 1)
            {
                secondDelta = (heightList[i] - heightList[i + 1]) * (1 - currentSpawnSettings.landSmooth);
            }

            heightList[i] += Mathf.Clamp((firstDelta + secondDelta) * 0.5f, currentSpawnSettings.minBgHeight - currentSpawnSettings.maxBgHeight, currentSpawnSettings.maxBgHeight - currentSpawnSettings.minBgHeight);
        }
    }

    public float SpawnRevivePlatform()
    {
        spawnQueue.Clear();
        spawnQueue.AddRange(SpawnSettingsHandler.ReviveSpawnQueue);

        return nextLinePositionX + 3;
    }
}