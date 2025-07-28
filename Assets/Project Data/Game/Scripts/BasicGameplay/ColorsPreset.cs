using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorsPreset", menuName = "ColorsSystem/ColorsPreset")]
public class ColorsPreset : ScriptableObject
{
    [Header("Platform settings")]
    public Color colorHigh = Color.white;
    public Color colorLow = Color.white;

    public float yPosHigh;
    public float yPosLow;

    public float gradientStrength;
    public float emissiveStrength;

    public Color colorX = Color.white;
    public Color colorY = Color.white;


    [Header("Spesial settings")]
    public Color jumpColor = Color.white;
    //public Color jumpEmissionColor = Color.white;
    public Color spikesColor = Color.white;
    //public Color spikesEmissionColor = Color.white;
    public Color invisiblePlatformColor = Color.white;
    public Color invesiblePlatformRimColor = Color.white;

    [Header("Background platform")]
    public Color colorHighBack = Color.white;
    public Color colorLowBack = Color.white;

    public float yPosHighBack;
    public float yPosLowBack;

    public float gradientStrengthBack;
    public float emissiveStrengthBack;

    public Color colorXBack = Color.white;
    public Color colorYBack = Color.white;


    [Header("Power ups")]
    public Color powerUpsMainColor = Color.white;
    public Color portalColor = Color.white;

    [Header("Sky")]
    public SkyColorType skyColorType;
    public Color cameraColor = Color.white;
    public Material skyboxMaterial;
    [Range(0, 7)]
    public float fogStrength;
    [Range(0.1f, 3)]
    public float fogSize;

    public enum SkyColorType
    {
        Skybox,
        Solid,
    }
}