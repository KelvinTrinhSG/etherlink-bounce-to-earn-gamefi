using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

[System.Serializable]
public class PlatformsSettingsContainer
{
    [Range(0f, 1f)]
    public float minDistanceRatio;
    [Range(0f, 1f)]
    public float maxDistanceRatio;

    public List<PlatformSpawnSettings> platfromsSpawnSettings = new List<PlatformSpawnSettings>();

    public QueueElement[] GetRandomPlatformSpawnQueue(float platformHeight)
    {
        PlatformSpawnSettings settings = GetRandomPlatformSettings();

        switch (settings.basicPlatformType)
        {
            case PlatformType.Mob:
                {
                    return GetMobPlatformSpawnQueue(settings, platformHeight);
                }
            case PlatformType.Moving:
                {
                    return GetMovingPlatformSpawnQueue(settings, platformHeight);
                }
            case PlatformType.Spiked:
                {
                    return GetSpikedPlatformSpawnQueue(settings, platformHeight);
                }
            default:
                {
                    return GetStandartPlatformSpawnQueue(settings, platformHeight);
                }
        }
    }

    private PlatformSpawnSettings GetRandomPlatformSettings()
    {
        int randomPercentage = Random.Range(1, 101);
        int valueToCheck = 0;

        for (int i = 0; i < platfromsSpawnSettings.Count; i++)
        {
            valueToCheck += platfromsSpawnSettings[i].platformSpawnPercentage;

            if (randomPercentage <= valueToCheck)
            {
                return platfromsSpawnSettings[i];
            }
        }

        Debug.LogError("Random percentage is out of bounds. Fix platforms spawn percentages. Outdone percentage : " + randomPercentage + ".");
        return platfromsSpawnSettings[0];
    }

    private QueueElement[] GetStandartPlatformSpawnQueue(PlatformSpawnSettings settings, float platformHeight)
    {
        int spaceLength = (int)(GetRandomDistanceRatio() * SpawnSettingsHandler.MaxAllowedPlatformsOffset);
        if (spaceLength + SpawnSettingsHandler.AdditionalSpaceBeforePlatform > SpawnSettingsHandler.MaxAllowedPlatformsOffset)
        {
            spaceLength = SpawnSettingsHandler.MaxAllowedPlatformsOffset - SpawnSettingsHandler.AdditionalSpaceBeforePlatform;
        }
        SpawnSettingsHandler.AdditionalSpaceBeforePlatform = 0;

        int basicPlatromsAmount = settings.GetRandomBasicPlatformsAmount();
        int extraPlatromsAmount = settings.GetRandomExtraPlatformsAmount();

        int basicPlatformsShift = Random.Range(0, extraPlatromsAmount + 1);

        QueueElement[] spawnQueue = new QueueElement[spaceLength + basicPlatromsAmount + extraPlatromsAmount];

        for (int i = 0; i < spaceLength + basicPlatromsAmount + extraPlatromsAmount; i++)
        {
            // initializing before platform space
            if (i < spaceLength)
            {
                spawnQueue[i] = new QueueElement(PlatformType.Shield, platformHeight, null);
            }
            // initializing basic platforms
            else if (i >= spaceLength + basicPlatformsShift && i < spaceLength + basicPlatformsShift + basicPlatromsAmount)
            {
                // initializing spacial spawn item
                SpecialSpawnItem specialSpawnItem = null;

                if (SpawnSettingsHandler.SpawnPowerUp)
                {
                    PowerUpType randomPowerUp = SpawnSettingsHandler.GetPowerupType();

                    //if (randomPowerUp == PowerUpType.CoinsMagnet)
                    //{
                    //    SpawnSettingsHandler.OnCoinsPowerUpIsSpawned();
                    //}

                    Vector3 additionalPosition = randomPowerUp == PowerUpType.EnergyField ? Vector3.zero.SetY(2f) : Vector3.zero.SetY(0.1f);

                    specialSpawnItem = new PowerUpSpawnSettings(randomPowerUp, additionalPosition);
                }
                //else if (SpawnSettingsHandler.SpawnCoin)
                //{
                //    specialSpawnItem = new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.Coin, Vector3.zero);
                //}


                spawnQueue[i] = new QueueElement(settings.basicPlatformType, platformHeight, specialSpawnItem);
            }
            // initializing extra platforms (which is a Normal platforms)
            else
            {
                spawnQueue[i] = new QueueElement(PlatformType.Normal, platformHeight, null);
            }
        }

        //string log = "space: " + spaceLength + " basic: " + basicPlatromsAmount + " extra: " + extraPlatromsAmount + " |";
        //for (int i = 0; i < spawnQueue.Length; i++)
        //{
        //    log += spawnQueue[i].platformID + "|";
        //}

        //Debug.Log(log);

        return spawnQueue;
    }

    private QueueElement[] GetSpikedPlatformSpawnQueue(PlatformSpawnSettings settings, float platformHeight)
    {
        int spaceLength = (int)(GetRandomDistanceRatio() * SpawnSettingsHandler.MaxAllowedPlatformsOffset);
        if (spaceLength + SpawnSettingsHandler.AdditionalSpaceBeforePlatform > SpawnSettingsHandler.MaxAllowedPlatformsOffset)
        {
            spaceLength = SpawnSettingsHandler.MaxAllowedPlatformsOffset - SpawnSettingsHandler.AdditionalSpaceBeforePlatform;
        }

        int basicPlatromsAmount = settings.GetRandomBasicPlatformsAmount();
        int extraPlatromsAmount = settings.GetRandomExtraPlatformsAmount();

        int basicPlatformsShift = Random.Range(0, extraPlatromsAmount + 1);


        // if theres no extra platforms before spikes and distance before them is to big than adding one extra platoform in front
        if (basicPlatformsShift == 0 && spaceLength + SpawnSettingsHandler.AdditionalSpaceBeforePlatform + basicPlatromsAmount > SpawnSettingsHandler.MaxAllowedPlatformsOffset)
        {
            basicPlatformsShift = 1;
            if (extraPlatromsAmount == 0)
            {
                extraPlatromsAmount = 1;
            }
        }

        SpawnSettingsHandler.AdditionalSpaceBeforePlatform = 0;

        QueueElement[] spawnQueue = new QueueElement[spaceLength + basicPlatromsAmount + extraPlatromsAmount];

        for (int i = 0; i < spaceLength + basicPlatromsAmount + extraPlatromsAmount; i++)
        {
            // initializing before platform space
            if (i < spaceLength)
            {
                spawnQueue[i] = new QueueElement(PlatformType.Shield, platformHeight, null);
            }
            // initializing basic platforms
            else if (i >= spaceLength + basicPlatformsShift && i < spaceLength + basicPlatformsShift + basicPlatromsAmount)
            {
                spawnQueue[i] = new QueueElement(settings.basicPlatformType, platformHeight, null);
            }
            // initializing extra platforms (which is a Normal platforms)
            else
            {
                // initializing spacial spawn item
                SpecialSpawnItem specialSpawnItem = null;

                if (SpawnSettingsHandler.SpawnPowerUp)
                {
                    PowerUpType randomPowerUp = (PowerUpType)Random.Range(0, PowerUp.powerUpsCount);

                    //if (randomPowerUp == PowerUpType.CoinsMagnet)
                    //{
                    //    SpawnSettingsHandler.OnCoinsPowerUpIsSpawned();
                    //}

                    Vector3 additionalPosition = randomPowerUp == PowerUpType.EnergyField ? Vector3.zero.SetY(2f) : Vector3.zero.SetY(0.1f);

                    specialSpawnItem = new PowerUpSpawnSettings(randomPowerUp, additionalPosition);
                }
                //else if (SpawnSettingsHandler.SpawnCoin)
                //{
                //    specialSpawnItem = new ObjectSpawnSettings(ObjectSpawnSettings.ObjectType.Coin, Vector3.zero);
                //}

                spawnQueue[i] = new QueueElement(PlatformType.Normal, platformHeight, specialSpawnItem);
            }
        }

        // if all extra platforms at spiked platform is on left side - than additional space before next platform will be equal to spiked platforms amount
        if (settings.basicPlatformType == PlatformType.Spiked && extraPlatromsAmount == basicPlatformsShift)
        {
            SpawnSettingsHandler.AdditionalSpaceBeforePlatform = basicPlatromsAmount;
        }

        //string log = "space: " + spaceLength + " basic: " + basicPlatromsAmount + " extra: " + extraPlatromsAmount + " |";
        //for (int i = 0; i < spawnQueue.Length; i++)
        //{
        //    log += spawnQueue[i].platformID + "|";
        //}

        //Debug.Log(log);

        return spawnQueue;
    }

    private QueueElement[] GetMobPlatformSpawnQueue(PlatformSpawnSettings settings, float platformHeight)
    {
        int spaceLength = (int)(GetRandomDistanceRatio() * SpawnSettingsHandler.MaxAllowedPlatformsOffset);
        if (spaceLength + SpawnSettingsHandler.AdditionalSpaceBeforePlatform > SpawnSettingsHandler.MaxAllowedPlatformsOffset)
        {
            spaceLength = SpawnSettingsHandler.MaxAllowedPlatformsOffset - SpawnSettingsHandler.AdditionalSpaceBeforePlatform;
        }
        SpawnSettingsHandler.AdditionalSpaceBeforePlatform = 0;

        int basicPlatromsAmount = settings.GetRandomBasicPlatformsAmount();
        int extraPlatromsAmount = settings.GetRandomExtraPlatformsAmount();

        int basicPlatformsShift = Random.Range(0, extraPlatromsAmount + 1);

        QueueElement[] spawnQueue = new QueueElement[spaceLength + basicPlatromsAmount + extraPlatromsAmount];

        for (int i = 0; i < spaceLength + basicPlatromsAmount + extraPlatromsAmount; i++)
        {
            // initializing before platform space
            if (i < spaceLength)
            {
                spawnQueue[i] = new QueueElement(PlatformType.Shield, platformHeight, null);
            }
            // initializing basic platforms
            else if (i >= spaceLength + basicPlatformsShift && i < spaceLength + basicPlatformsShift + basicPlatromsAmount)
            {
                spawnQueue[i] = new QueueElement(settings.basicPlatformType, platformHeight, null, Random.Range(0, 2) == 0);

                if (i == spaceLength + basicPlatformsShift)
                {
                    spawnQueue[i].specialSpawnItem = new MobSpawnSettings(basicPlatromsAmount);
                }
            }
            // initializing extra platforms (which is a Normal platforms)
            else
            {
                spawnQueue[i] = new QueueElement(PlatformType.Normal, platformHeight, null);
            }
        }

        return spawnQueue;
    }

    private QueueElement[] GetMovingPlatformSpawnQueue(PlatformSpawnSettings settings, float platformHeight)
    {
        int spaceLength = (int)(GetRandomDistanceRatio() * SpawnSettingsHandler.MaxAllowedPlatformsOffset);

        int basicPlatromsAmount = settings.GetRandomBasicPlatformsAmount();
        int movingSpaceLength = settings.GetRandomExtraPlatformsAmount();

        if (spaceLength + SpawnSettingsHandler.AdditionalSpaceBeforePlatform + movingSpaceLength > SpawnSettingsHandler.MaxAllowedPlatformsOffset)
        {
            spaceLength = SpawnSettingsHandler.MaxAllowedPlatformsOffset - (SpawnSettingsHandler.AdditionalSpaceBeforePlatform + movingSpaceLength);

            // if we can't compensate SpawnSettings.AdditionalSpaceBeforePlatform by decreasing spaceLength
            // then we need to decreace moving SpaceLength 
            if (spaceLength < 1)
            {
                movingSpaceLength += spaceLength - 1;
                spaceLength = 1;

                // if platform is not able to move make it normal
                if (movingSpaceLength <= 0)
                {
                    QueueElement[] normalPlatformQueue = new QueueElement[basicPlatromsAmount];

                    for (int i = 0; i < basicPlatromsAmount; i++)
                    {
                        normalPlatformQueue[i] = new QueueElement((int)PlatformType.Normal, platformHeight, null);
                    }

                    SpawnSettingsHandler.AdditionalSpaceBeforePlatform = 0;
                    return normalPlatformQueue;
                }
            }
        }

        SpawnSettingsHandler.AdditionalSpaceBeforePlatform = movingSpaceLength + 1; // adding +1 to make space beetween platforms at min 1 cell

        QueueElement[] spawnQueue = new QueueElement[spaceLength + basicPlatromsAmount + movingSpaceLength];

        MovingPlatformSpawnSettings movePlatformSpawnSettings = new MovingPlatformSpawnSettings(basicPlatromsAmount, movingSpaceLength);

        for (int i = 0; i < spaceLength + basicPlatromsAmount + movingSpaceLength; i++)
        {
            // initializing before platform space
            if (i < spaceLength)
            {
                spawnQueue[i] = new QueueElement(PlatformType.Shield, platformHeight, null);
            }
            // initializing moving platforms
            else if (i < spaceLength + basicPlatromsAmount)
            {
                spawnQueue[i] = new QueueElement(settings.basicPlatformType, platformHeight, movePlatformSpawnSettings);
            }
            // initializing moving space (air)
            else
            {
                spawnQueue[i] = new QueueElement(PlatformType.Shield, platformHeight, null);
            }
        }

        return spawnQueue;
    }

    public float GetRandomDistanceRatio()
    {
        if (minDistanceRatio > maxDistanceRatio)
        {
            maxDistanceRatio = minDistanceRatio;
        }

        return Random.Range(minDistanceRatio, maxDistanceRatio);
    }
}