using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class MobBehaviour : MonoBehaviour
{
    public float movingSpeed = 0.5f;

    private bool active = false;

    private float minPositionX;
    private float maxPositionX;
    private float speedSign;

    private Transform disablerTransform;

    private void Start()
    {
        disablerTransform = SpawnController.instance.disablerTransform;
    }

    public void Init(MobSpawnSettings mobSpawnSettings, float leftPosition)
    {
        //Debug.Log(name + "  mss: " + mobSpawnSettings.groundPlatformsAmount);
        minPositionX = leftPosition;
        maxPositionX = minPositionX + mobSpawnSettings.groundPlatformsAmount - 1;

        transform.position = transform.position.SetX(Random.Range(minPositionX, maxPositionX));

        speedSign = Random.Range(0,2) == 0 ? 1 : -1;
        transform.localScale = Vector3.one.SetX(speedSign);

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
                transform.localScale = Vector3.one.SetX(speedSign);
            }
        }

        if (transform.position.x < disablerTransform.position.x)
        {
            active = false;
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        active = false;
    }
}
