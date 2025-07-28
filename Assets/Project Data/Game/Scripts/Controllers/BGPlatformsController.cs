using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGPlatformsController : MonoBehaviour
{
    public static BGPlatformsController instance;

    private List<BackgroundPlatformData> backgroundPlatformDatas =  new List<BackgroundPlatformData>();


    private bool randomMovementActive;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        randomMovementActive = true;
    }


    public void AddPlatform(BackgroundPlatformData platformData)
    {
        backgroundPlatformDatas.Add(platformData);
    }

    public void RemovePlatform(BackgroundPlatformData platformData)
    {
        backgroundPlatformDatas.Remove(platformData);
    }

    private void FixedUpdate()
    {
        if(randomMovementActive)
        {
            for (int i = 0; i < backgroundPlatformDatas.Count; i++)
            {
                backgroundPlatformDatas[i].UpdatePosition();
            }
        }
    }
}