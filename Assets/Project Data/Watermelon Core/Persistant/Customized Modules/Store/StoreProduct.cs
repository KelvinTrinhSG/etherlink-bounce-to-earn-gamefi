using System.Collections.Generic;
using UnityEngine;

public abstract class StoreProduct : ScriptableObject
{
    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public string productName;

    public Sprite icon;

    public GameObject skinPrefab;
    public Color hitEffectColor = Color.white;

    public PlayerController.RotationMode rotationMode;

    [Header("Overrided values")]
    public ParticleSystem specialHitEffectPrefab;
    public GameObject trailPrefab;
    public ColorsPreset backgroundColorsPreset;

    public AudioClip[] hitSounds;
    public AudioClip gameOverSound;

    protected BehaviourType productBehaviourType;
    public BehaviourType ProductBehaviourType
    {
        get { return productBehaviourType; }
    }

    public virtual void Init() { }
    public virtual bool IsActive() { return true; }
    public virtual bool IsOpened() { return false; }
    public virtual bool Check() { return false; }
    public virtual void Buy() { }

    public virtual float Progress()
    {
        return 0;
    }

}

public enum BehaviourType
{
    Default = 0,
    Achievement = 1,
    Ads = 2,
    Share = 3
}