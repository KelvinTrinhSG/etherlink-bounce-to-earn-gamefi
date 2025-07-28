using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsController : MonoBehaviour
{
    public static ColorsController instance;

    [Header("Settings")]
    public float presetTransitionTime = 1f;
    public List<SkinPresetsContainer> skinsPresetsIgnoreList;

    [Header("References")]
    public Material backgroundPlatformMaterial;
    public Material platfromMaterial;
    public Material jumpPlatformMaterial;
    public Material spikedPlatformMaterial;
    public Material hologramMaterial;
    public Material powerUpsMaterial;
    public Material invisiblePlatformMaterial;

    [Space(5)]
    public Camera cameraRef;
    public SpawnController spawnController;

    [Space(5)]
    public List<ColorsPreset> levelPresetsQueue;
    public ColorsPreset ultraComboPreset;
    public ColorsPreset tutorialPreset;
    
    private ColorsPreset currentPreset;
    private ColorsPreset levelPreset;
    private ColorsPreset prevPreset;

    private int currentLevelPresetIndex;

    public static ColorsPreset CurrentPreset
    {
        get { return instance.currentPreset; }
    }

    private void Awake()
    {
        instance = this;

        currentLevelPresetIndex = 0;
        currentPreset = levelPreset = prevPreset = GetStartPreset();

        if(levelPreset != null)
            SetPreset(levelPreset);
    }

    private ColorsPreset GetStartPreset()
    {
        if(spawnController != null)
        {
            if (spawnController.spawnSetings.levels[0].levelPresets[0].preferedPreset != null)
            {
                return spawnController.spawnSetings.levels[0].levelPresets[0].preferedPreset;
            }
            else
            {
                return levelPresetsQueue[currentLevelPresetIndex];
            }
        }

        return null;
    }

    public static void SetNextLevelPreset(ColorsPreset preferedPreset)
    {
        if (preferedPreset != null)
        {
            instance.prevPreset = instance.levelPreset;
            instance.levelPreset = preferedPreset;
        }
        else
        {
            instance.prevPreset = instance.levelPreset;
            instance.UpdateCurrentPresetIndex();
            instance.levelPreset = instance.levelPresetsQueue[instance.currentLevelPresetIndex];
        }

        if (PlayerController.ComboState != ComboState.Ultra)
        {
            LerpToPreset(instance.levelPreset);
        }
    }

    private void UpdateCurrentPresetIndex()
    {
        currentLevelPresetIndex++;

        if (currentLevelPresetIndex >= levelPresetsQueue.Count)
        {
            currentLevelPresetIndex = 0;
        }
        
        if (levelPresetsQueue[currentLevelPresetIndex] == prevPreset)
        {
            currentLevelPresetIndex++;

            if (currentLevelPresetIndex >= levelPresetsQueue.Count)
            {
                currentLevelPresetIndex = 0;
            }
        }
    }

    public static void SetPreset(ColorsPreset colorPreset)
    {
        instance.currentPreset = colorPreset;
        instance.SetColorPreset(instance.currentPreset);
    }

    private void SetColorPreset(ColorsPreset colorPreset)
    {
        // background platform
        backgroundPlatformMaterial.SetColor("_ColorLow", colorPreset.colorLowBack);
        backgroundPlatformMaterial.SetColor("_ColorHigh", colorPreset.colorHighBack);
        backgroundPlatformMaterial.SetColor("_ColorX", colorPreset.colorXBack);
        backgroundPlatformMaterial.SetColor("_ColorY", colorPreset.colorYBack);
        backgroundPlatformMaterial.SetFloat("_yPosLow", colorPreset.yPosLowBack);
        backgroundPlatformMaterial.SetFloat("_yPosHigh", colorPreset.yPosHighBack);
        backgroundPlatformMaterial.SetFloat("_GradientStrength", colorPreset.gradientStrengthBack);
        backgroundPlatformMaterial.SetFloat("_EmissiveStrengh", colorPreset.emissiveStrengthBack);
        
        // platform
        platfromMaterial.SetColor("_ColorLow", colorPreset.colorLow);
        platfromMaterial.SetColor("_ColorHigh", colorPreset.colorHigh);
        platfromMaterial.SetColor("_ColorX", colorPreset.colorX);
        platfromMaterial.SetColor("_ColorY", colorPreset.colorY);
        platfromMaterial.SetFloat("_yPosLow", colorPreset.yPosLow);
        platfromMaterial.SetFloat("_yPosHigh", colorPreset.yPosHigh);
        platfromMaterial.SetFloat("_GradientStrength", colorPreset.gradientStrength);
        platfromMaterial.SetFloat("_EmissiveStrengh", colorPreset.emissiveStrength);
        
        // special platforms
        jumpPlatformMaterial.SetColor("_Color", colorPreset.jumpColor);
        spikedPlatformMaterial.SetColor("_Color", colorPreset.spikesColor);

        invisiblePlatformMaterial.SetColor("_MainColor", colorPreset.invisiblePlatformColor);
        invisiblePlatformMaterial.SetColor("_RimColor", colorPreset.invesiblePlatformRimColor);

        powerUpsMaterial.SetColor("_Color", colorPreset.powerUpsMainColor);

        if (colorPreset.skyColorType == ColorsPreset.SkyColorType.Skybox && colorPreset.skyboxMaterial != null)
        {
            RenderSettings.skybox = colorPreset.skyboxMaterial;
            cameraRef.clearFlags = CameraClearFlags.Skybox;
        }
        else
        {
            cameraRef.clearFlags = CameraClearFlags.SolidColor;
            cameraRef.backgroundColor = colorPreset.cameraColor;
        }

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 10 - colorPreset.fogSize;
        RenderSettings.fogEndDistance = 17 - colorPreset.fogStrength;
        RenderSettings.fogColor = colorPreset.cameraColor;
    }

    public static void LerpToPreset(ColorsPreset colorPreset)
    {
        instance.currentPreset = colorPreset;
        instance.StartCoroutine(instance.PresetChangeCorountine(instance.currentPreset));
    }


    private IEnumerator PresetChangeCorountine(ColorsPreset preset)
    {
        Color startColorLow = platfromMaterial.GetColor("_ColorLow");
        Color startColorHigh = platfromMaterial.GetColor("_ColorHigh");
        Color startColorX = platfromMaterial.GetColor("_ColorX");
        Color startColorY = platfromMaterial.GetColor("_ColorY");
        float startYPosLow = platfromMaterial.GetFloat("_yPosLow");
        float startYPosHigh = platfromMaterial.GetFloat("_yPosHigh");
        float startGradientStrength = platfromMaterial.GetFloat("_GradientStrength");
        float startEmissiveStrength = platfromMaterial.GetFloat("_EmissiveStrengh");

        Color startColorJump = jumpPlatformMaterial.GetColor("_Color");
        Color startColorSpikes = spikedPlatformMaterial.GetColor("_Color");

        Color startColorInvisible = invisiblePlatformMaterial.GetColor("_MainColor");
        Color startColorInvisibleRim = invisiblePlatformMaterial.GetColor("_RimColor");
        
        Color startColorLowB = backgroundPlatformMaterial.GetColor("_ColorLow");
        Color startColorHighB = backgroundPlatformMaterial.GetColor("_ColorHigh");
        Color startColorXB = backgroundPlatformMaterial.GetColor("_ColorX");
        Color startColorYB = backgroundPlatformMaterial.GetColor("_ColorY");
        float startYPosLowB = backgroundPlatformMaterial.GetFloat("_yPosLow");
        float startYPosHighB = backgroundPlatformMaterial.GetFloat("_yPosHigh");
        float startGradientStrengthB = backgroundPlatformMaterial.GetFloat("_GradientStrength");
        float startEmissiveStrengthB = backgroundPlatformMaterial.GetFloat("_EmissiveStrengh");

        Color startCameraColor = cameraRef.backgroundColor;
        float startFogEndDistance = RenderSettings.fogEndDistance;

        float timer = 0f;

        while (timer < presetTransitionTime)
        {
            platfromMaterial.SetColor("_ColorLow", Color.Lerp(startColorLow, preset.colorLow, timer));
            platfromMaterial.SetColor("_ColorHigh", Color.Lerp(startColorHigh, preset.colorHigh, timer));
            platfromMaterial.SetColor("_ColorX", Color.Lerp(startColorX, preset.colorX, timer));
            platfromMaterial.SetColor("_ColorY", Color.Lerp(startColorY, preset.colorY, timer));
            platfromMaterial.SetFloat("_yPosLow", Mathf.Lerp(startYPosLow, preset.yPosLow, timer));
            platfromMaterial.SetFloat("_yPosHigh", Mathf.Lerp(startYPosHigh, preset.yPosHigh, timer));
            platfromMaterial.SetFloat("_GradientStrength", Mathf.Lerp(startGradientStrength, preset.gradientStrength, timer));
            platfromMaterial.SetFloat("_EmissiveStrengh", Mathf.Lerp(startEmissiveStrength, preset.emissiveStrength, timer));
            
            jumpPlatformMaterial.SetColor("_Color", Color.Lerp(startColorJump, preset.jumpColor, timer));
            spikedPlatformMaterial.SetColor("_Color", Color.Lerp(startColorSpikes, preset.spikesColor, timer));

            invisiblePlatformMaterial.SetColor("_MainColor", Color.Lerp(startColorInvisible, preset.invisiblePlatformColor, timer));
            invisiblePlatformMaterial.SetColor("_RimColor", Color.Lerp(startColorInvisibleRim, preset.invesiblePlatformRimColor, timer));
            
            backgroundPlatformMaterial.SetColor("_ColorLow", Color.Lerp(startColorLowB, preset.colorLowBack, timer));
            backgroundPlatformMaterial.SetColor("_ColorHigh", Color.Lerp(startColorHighB, preset.colorHighBack, timer));
            backgroundPlatformMaterial.SetColor("_ColorX", Color.Lerp(startColorXB, preset.colorXBack, timer));
            backgroundPlatformMaterial.SetColor("_ColorY", Color.Lerp(startColorYB, preset.colorYBack, timer));
            backgroundPlatformMaterial.SetFloat("_yPosLow", Mathf.Lerp(startYPosLowB, preset.yPosLowBack, timer));
            backgroundPlatformMaterial.SetFloat("_yPosHigh", Mathf.Lerp(startYPosHighB, preset.yPosHighBack, timer));
            backgroundPlatformMaterial.SetFloat("_GradientStrength", Mathf.Lerp(startGradientStrengthB, preset.gradientStrengthBack, timer));
            backgroundPlatformMaterial.SetFloat("_EmissiveStrengh", Mathf.Lerp(startEmissiveStrengthB, preset.emissiveStrengthBack, timer));

            cameraRef.backgroundColor = Color.Lerp(startCameraColor, preset.cameraColor, timer);
            RenderSettings.fogColor = cameraRef.backgroundColor;
            RenderSettings.fogEndDistance = Mathf.Lerp(startFogEndDistance, 20 - preset.fogStrength, timer);

            timer += Time.deltaTime;
            yield return null;
        }

        SetColorPreset(preset);
    }

    public static void SetUltraPreset()
    {
        LerpToPreset(instance.ultraComboPreset);
    }

    public static void OnUltraComboBroke()
    {
        LerpToPreset(instance.levelPreset);
    }
    
    [System.Serializable]
    public class SkinPresetsContainer
    {
        public StoreSkinProduct skin;
        public List<ColorsPreset> presets;
    }
}