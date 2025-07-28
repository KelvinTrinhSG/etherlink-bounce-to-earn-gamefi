using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ReferenceController : MonoBehaviour
{
    public static ReferenceController instance;

    [Header("Controllers")]
    public GameController gameController;

    public UIControllerGame uiController;
    public SpawnController spawnController;
    public PlayerController playerController;

    [Header("Deffault objects")]
    public ParticleSystem defaultHitEffect;
    public GameObject defaultTrailObject;

    [Header ("Sounds")]
    public AudioMixer musicEffectsMixer;

    [Space(5)]
    public Material normalPlatformMaterial;
    public Material comboPlatformMaterial;
    public Material invisiblePlatformMaterial;
      
    private void Awake()
    {

#if UNITY_EDITOR
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of ReferenceController");
        }
#else
        instance = this;
#endif

    }
}
