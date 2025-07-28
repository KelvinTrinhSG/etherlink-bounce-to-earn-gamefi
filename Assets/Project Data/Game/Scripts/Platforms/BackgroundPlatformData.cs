using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class BackgroundPlatformData : MonoBehaviour
{
    public float movingSpeed;
    public float maxOffset = 1f;

    private float initialPlatformHeight;
    private float state;
    private float stateDelta;
    private int sign;

    public void StartMovement()
    {
        initialPlatformHeight = transform.position.y;
        state = 0.5f;
        sign = Random.Range(0, 2) == 0 ? 1 : -1;

        stateDelta = Time.fixedDeltaTime / maxOffset * 2 * movingSpeed;

        BGPlatformsController.instance.AddPlatform(this);
    }

    public void UpdatePosition()
    {
        state += stateDelta * sign;

        if (state > 1 || state < -1)
        {
            sign *= -1;
        }

        transform.position = transform.position.SetY(maxOffset * state + initialPlatformHeight);
    }

    private void OnDisable()
    {
        if (BGPlatformsController.instance != null)
        {
            BGPlatformsController.instance.RemovePlatform(this);
        }
    }
}
