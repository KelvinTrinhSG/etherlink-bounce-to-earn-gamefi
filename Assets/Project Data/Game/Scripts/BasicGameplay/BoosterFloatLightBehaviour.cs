using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class BoosterFloatLightBehaviour : MonoBehaviour
{
    public static BoosterFloatLightBehaviour instance;

    private Transform transformRef;
    private Transform cameraTransform;
    public Rigidbody2D playerRigidbody;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        gameObject.SetActive(false);

        transformRef = transform;
        cameraTransform = CameraController.instance.transform;
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        //Tween.DelayedCall(PowerUp.boosterDuration * 2, () => gameObject.SetActive(false));
    }

    private void Update()
    {
        if (playerRigidbody.velocity.x > 18f)
        {
            transformRef.position = cameraTransform.position.SetZ(2);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
