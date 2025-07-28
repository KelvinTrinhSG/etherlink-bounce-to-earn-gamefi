using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class MovingPlatformBehaviour : MonoBehaviour
{
    public static List<MovingPlatformBehaviour> currentlySpawningMovingPlatform = new List<MovingPlatformBehaviour>();

    public float movingSpeed = 0.5f;

    private bool active = false;
    private bool isFirstPlarform = false;

    private float minPositionX;
    private float maxPositionX;
    private float speedSign;

    private GameObject[] guideLines = new GameObject[3];

    public void Init(MovingPlatformSpawnSettings settings)
    {
        minPositionX = transform.position.x;
        maxPositionX = minPositionX + settings.movingSpaceLength;

        speedSign = 1;
        isFirstPlarform = false;

        currentlySpawningMovingPlatform.Add(this);

        if (!settings.inited)
        {
            guideLines[0] = PoolManager.GetPoolByName("LeftGuideline").GetPooledObject(new Vector3((minPositionX - 1), -1f));
            guideLines[1] = PoolManager.GetPoolByName("CentralGuideline").GetPooledObject(new Vector3(minPositionX + ((maxPositionX + settings.platformsAmount - minPositionX) / 2f) - 0.5f, -1f));
            guideLines[1].transform.localScale = new Vector3(settings.movingSpaceLength + settings.platformsAmount, 1, 1);
            guideLines[2] = PoolManager.GetPoolByName("RightGuideline").GetPooledObject(new Vector3(maxPositionX + settings.platformsAmount, -1f));

            settings.inited = true;
            isFirstPlarform = true;
        }

        int movingPlatformCount = currentlySpawningMovingPlatform.Count;
        if (movingPlatformCount >= settings.platformsAmount)
        {
            for (int i = 0; i < movingPlatformCount; i++)
            {
                currentlySpawningMovingPlatform[i].ActivatePlatform();
            }
            currentlySpawningMovingPlatform.Clear();
        }
    }

    public void ActivatePlatform()
    {
        active = true;
    }

    private void Update()
    {
        if (active)
        {
            if ((speedSign == 1 && transform.position.x < maxPositionX) || (speedSign == -1 && transform.position.x > minPositionX))
            {
                transform.position = transform.position.AddToX(movingSpeed * speedSign * Time.deltaTime);
            }
            else
            {
                speedSign *= -1f;
            }
        }
    }

    private void OnDisable()
    {
        active = false;

        if (isFirstPlarform)
        {
            guideLines[0].SetActive(false);
            guideLines[1].SetActive(false);
            guideLines[2].SetActive(false);
        }
    }
}
