using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Watermelon;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    private const float relaxZoneZCoef = -0.282028f;

    [Header("Settings")]
    public float smooth = 3f;
    public float normalOffsetX = 6.5f;
    public float normalOffsetY = 3.5f;
    public float normalOffsetZ = -9f;
    public float relaxOffsetY = -3.2f;
    public float relaxOfsetZ = -5.5f;

    [Space(5)]
    public float speedDependMinZAddition = 0.3f;
    public float speedDependMaxZAddition = -1f;

    [Space(5)]
    public Vector3 menuAdditionaOffset;
    public Vector3 storeAdditionaOffset;

    [Header("References")]
    public Transform target;
    public Camera cameraRef;
    public Disabler disabler;
    public CameraShake cameraShake;

    private Transform transformRef;
    private Vector3 destination;
    public Vector3 currentAdditionalOffset;
    private TweenCase cameraEstrangeTween;
    
    private bool isEnabled = true;
    private bool preparedToExitRelaxMode;
    private bool isGameMode = false;

    private float relaxZoneXLine;
    private float cameraViewWidth;
    private float speedEstrangeValue;
    
    private void Awake()
    {
        transformRef = transform;

        instance = this;

        InitializeAspectRatioSettings();

        OnMenuOpened();
        transformRef.position = new Vector3(normalOffsetX, normalOffsetY, normalOffsetZ + speedDependMinZAddition) + currentAdditionalOffset;
    }

    void Start()
    {
        isEnabled = true;

        destination = transform.position;
    }

    void Update()
    {
        if (!isEnabled)
            return;

        destination = new Vector3(target.position.x + normalOffsetX, normalOffsetY, normalOffsetZ);

        if (isGameMode)
        {
            speedEstrangeValue = Mathf.Lerp(speedEstrangeValue, Mathf.Lerp(speedDependMinZAddition, speedDependMaxZAddition, Mathf.Clamp01(PlayerController.Velocity.x / 20f)), 0.2f); // 20 is max player velocity value
            destination = destination.SetZ(normalOffsetZ + speedEstrangeValue);
        }
        else
        {
            destination += currentAdditionalOffset;
        }

        transformRef.position = Vector3.Lerp(transformRef.position, destination, smooth * Time.deltaTime);
    }

    public static void OnMenuOpened()
    {
        instance.currentAdditionalOffset = instance.menuAdditionaOffset;
        instance.isGameMode = false;
    }

    public static void OnStoreOpened()
    {
        instance.currentAdditionalOffset = instance.storeAdditionaOffset;
        instance.isGameMode = false;
    }

    public static void OnGameStarted()
    {
        instance.currentAdditionalOffset = Vector3.zero;
        instance.isGameMode = true;
    }

    #region Aspect Ratio based settings

    [System.Serializable]
    public class AspectRatioSettings
    {
        public string ratio;
        public float aspectRatio;

        [Space(5)]
        public float disablerOffsetX;
        public float normalOffsetX;
        public float normalOffsetZ;
        public float relaxOffsetZ;

        public AspectRatioSettings()
        {
            ratio = "new ratio";
            aspectRatio = 0f;

            disablerOffsetX = 0f;
            normalOffsetX = 0f;
            normalOffsetZ = 0f;
            relaxOffsetZ = 0f;
        }

        public static AspectRatioSettings GetInterpolatedSettings(AspectRatioSettings smallerSettings, AspectRatioSettings biggerSettings, float interpolationCoef)
        {
            AspectRatioSettings newSettins = new AspectRatioSettings();

            newSettins.disablerOffsetX = Mathf.Lerp(smallerSettings.disablerOffsetX, biggerSettings.disablerOffsetX, interpolationCoef);
            newSettins.normalOffsetX = Mathf.Lerp(smallerSettings.normalOffsetX, biggerSettings.normalOffsetX, interpolationCoef);
            newSettins.normalOffsetZ = Mathf.Lerp(smallerSettings.normalOffsetZ, biggerSettings.normalOffsetZ, interpolationCoef);

            newSettins.relaxOffsetZ = Mathf.Lerp(smallerSettings.relaxOffsetZ, biggerSettings.relaxOffsetZ, interpolationCoef);


            return newSettins;
        }
    }

    public AspectRatioSettings[] aspectRatiosSettings;

    private void InitializeAspectRatioSettings()
    {
        aspectRatiosSettings = aspectRatiosSettings.OrderByDescending(x => x.aspectRatio).ToArray();

        float currentAspectRatio = cameraRef.aspect;

        for (int i = 0; i < aspectRatiosSettings.Length; i++)
        {
            // if our aspect ratio is spesified at settings - perfect - applying settings
            if (currentAspectRatio == aspectRatiosSettings[i].aspectRatio)
            {
                ApplyAspectRatioSettings(aspectRatiosSettings[i]);
                break;
            }
            // if current aspect is bigger  than current saved settings aspect - we have not this aspect saved - interpolating beetween current and previous settings
            else if (currentAspectRatio > aspectRatiosSettings[i].aspectRatio)
            {
                // if biggest saved setting aspect is smaller than we need - anyway applying this settings
                if (i == 0)
                {
                    ApplyAspectRatioSettings(aspectRatiosSettings[i]);
                    break;
                }
                // otherwise interpolating settings
                else
                {
                    float interpolationCoef = (currentAspectRatio - aspectRatiosSettings[i].aspectRatio) / (aspectRatiosSettings[i - 1].aspectRatio - aspectRatiosSettings[i].aspectRatio);

                    AspectRatioSettings interpolatedSettings = AspectRatioSettings.GetInterpolatedSettings(aspectRatiosSettings[i], aspectRatiosSettings[i - 1], interpolationCoef);

                    ApplyAspectRatioSettings(interpolatedSettings);
                    break;
                }
            }
            // if current aspect is smaller then smallest saved settings - applying smallest settings
            else if (i == aspectRatiosSettings.Length - 1)
            {
                ApplyAspectRatioSettings(aspectRatiosSettings[i]);
                break;
            }
            // otherwise moving to next settings
        }
    }

    private void ApplyAspectRatioSettings(AspectRatioSettings settings)
    {
        disabler.cameraOffset = disabler.cameraOffset.SetX(settings.disablerOffsetX);
        normalOffsetX = settings.normalOffsetX;
        normalOffsetZ = settings.normalOffsetZ;

        relaxOfsetZ = settings.relaxOffsetZ;
    }

    #endregion

}