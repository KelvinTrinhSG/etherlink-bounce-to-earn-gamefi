using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class Platform : MonoBehaviour
{
    public PlatformType type;
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public GameObject spikesObject;

    //[HideInInspector]
    public bool isComboPlatform = false;

    protected MaterialPropertyBlock propertyBlock;
    private List<GameObject> platformParts = new List<GameObject>();
    private Transform disablerTransform;
    private BoxCollider2D boxCollider;
    private Pool comboTriggerPool;
    private Pool spikesBreakPool;
    private ComboTrigger comboTrigger;
    private Platform startColumnRef;
    private MobBehaviour mobRef;


    private bool supportsApperaingZoneAnimation = false;
    private bool appearingZoneAnimationActive = false;
    private bool skipFirstOnEnable = true;
    private bool isComboAnimationAvailable = true;
    private float appearingZoneNormalHeight;
    private float appearingZoneStartHeight = -4.5f;
    private float appearingZoneAnimFinishDifference = 5f;
    private float appearingZoneAnimStartDifference = 10f;
    private float appearingZoneAnimOneDivByDifferenceDelta;

    private Ease.EaseFunction appearingEasingFunction;

    protected virtual void Awake()
    {
        supportsApperaingZoneAnimation = !name.Contains("Back");

        if (type != PlatformType.Background || type != PlatformType.Shield)
            boxCollider = gameObject.GetComponent<BoxCollider2D>();

        appearingEasingFunction = Ease.GetFunction(Ease.Type.CubicIn);
        propertyBlock = new MaterialPropertyBlock();
    }

    protected virtual void Start()
    {
        appearingZoneAnimOneDivByDifferenceDelta = 1 / (appearingZoneAnimStartDifference - appearingZoneAnimFinishDifference);

        disablerTransform = SpawnController.instance.disablerTransform;
    }

    protected virtual void OnEnable()
    {
        if (skipFirstOnEnable)
        {

            skipFirstOnEnable = false;
            return;
        }

        if (supportsApperaingZoneAnimation && SpawnSettingsHandler.CurrentZone == Zone.Appearing)
        {
            appearingZoneNormalHeight = transform.position.y;
            transform.position = transform.position.SetY(appearingZoneStartHeight);

            appearingZoneAnimationActive = true;
        }

        isComboAnimationAvailable = true;

        if (comboTriggerPool == null)
        {
            comboTriggerPool = PoolManager.GetPoolByName("ComboTrigger");
            spikesBreakPool = PoolManager.GetPoolByName("SpikesBreakParticle");
        }
    }

    protected virtual void Update()
    {
        if (transform.position.x < disablerTransform.position.x)
        {
            DisablePlatform();
        }

        // appearing zone animation
        if (!supportsApperaingZoneAnimation || !appearingZoneAnimationActive)
            return;

        float difference = Mathf.Clamp(transform.position.x - PlayerController.Position.x, appearingZoneAnimFinishDifference, appearingZoneAnimStartDifference);

        if (difference == appearingZoneAnimStartDifference)
            return;

        transform.position = transform.position.SetY(Mathf.Lerp(appearingZoneNormalHeight, appearingZoneStartHeight, appearingEasingFunction((difference - appearingZoneAnimFinishDifference) * appearingZoneAnimOneDivByDifferenceDelta)));

        if (difference == appearingZoneAnimFinishDifference)
        {
            appearingZoneAnimationActive = false;
        }
    }

    protected virtual void DisablePlatform()
    {
        if (type != PlatformType.Background && type != PlatformType.Shield)
        {
            boxCollider.enabled = false;
        }

        if(type == PlatformType.Spiked)
        {
            spikesObject.SetActive(true);
        }

        if (isComboPlatform)
        {
            isComboPlatform = false;
            DisableComboLook();

            if (comboTrigger != null)
            {
                comboTrigger.FastDisable();
            }
        }

        platformParts.Clear();

        gameObject.SetActive(false);
    }

    public void InitializeCombo()
    {
        isComboPlatform = true;
        comboTrigger = comboTriggerPool.GetPooledObject(transform.position).GetComponent<ComboTrigger>();
        comboTrigger.Initialize(this);

        //SetEmissionPropertyValue(1.5f, meshRenderer);
    }

    public void DisableComboLook(bool manualDisable = false)
    {
        if (!isComboPlatform)
            return;

        isComboPlatform = false;

        if(manualDisable)
        {
            comboTrigger.Disable();
        }
    }

    public void RunComboAnimation(ComboState comboState)
    {
        if(platformParts != null && platformParts.Count > 0 )
        {
            StartComboAnimation(comboState);
        }
        // when player hits part of platform after spikes - platform whith collider does not have refs to platform parts - so calling this on start platform
        else if(startColumnRef != null)
        {
            startColumnRef.RunComboAnimation(comboState);
        }
    }

    private void StartComboAnimation(ComboState comboState)
    {
        if (isComboAnimationAvailable)
        {
            int startPlatformIndex = (int)(PlayerController.Position.x - boxCollider.bounds.min.x);
            StartCoroutine(ComboAnimationController(startPlatformIndex, comboState == ComboState.Ultra));

            isComboAnimationAvailable = false;
        }
    }

    private IEnumerator ComboAnimationController(int animationStartIndex, bool isUltra)
    {
        animationStartIndex = Mathf.Clamp(animationStartIndex, 0, platformParts.Count - 1);

        PlatformComboAnimation(platformParts[animationStartIndex].transform, isUltra);

        int leftIndex = animationStartIndex - 1;
        int rightIndex = animationStartIndex + 1;

        float delay = 0.06f;

        while (leftIndex >= 0 || rightIndex < platformParts.Count)
        {
            yield return new WaitForSeconds(delay);

            if (leftIndex >= 0)
            {
                PlatformComboAnimation(platformParts[leftIndex].transform, isUltra);
                leftIndex--;
            }

            if (rightIndex < platformParts.Count)
            {
                PlatformComboAnimation(platformParts[rightIndex].transform, isUltra);
                rightIndex++;
            }
        }
    }

    private void PlatformComboAnimation(Transform animatedPlatformTransform, bool isUltra)
    {
        float startHeight = animatedPlatformTransform.position.y;
        float offset = isUltra ? 0.32f : 0.12f;

        animatedPlatformTransform.transform.DOMoveY(startHeight - offset, 0.1f).OnComplete(() =>
        {
            animatedPlatformTransform.transform.DOMoveY(startHeight, 0.1f);
        });

        if(isUltra && animatedPlatformTransform.name.Contains("Spiked"))
        {
            animatedPlatformTransform.GetComponent<Platform>().DestroySpikes();
        }
        else if(isUltra && type == PlatformType.Mob && mobRef != null && Mathf.Abs( mobRef.transform.position.x - animatedPlatformTransform.position.x) <= 0.7f)
        {
            mobRef.gameObject.SetActive(false);
            spikesBreakPool.GetPooledObject(animatedPlatformTransform.position);
            mobRef = null;
        }
    }

    public virtual void AddPlatformPart(GameObject platform, bool isItSpawnPlatformPart)
    {
        platformParts.Add(platform);

        // spiked platform has multiple colliders - ensure that all colliders has ref to start column
        if(isItSpawnPlatformPart)
        {
            platform.GetComponent<Platform>().startColumnRef = this;
        }
    }

    public void DestroySpikes()
    {
        if (type != PlatformType.Spiked)
            return;

        spikesObject.SetActive(false);
        spikesBreakPool.GetPooledObject(transform.position);
    }

    public void SetMobReference(MobBehaviour mob)
    {
        if (type != PlatformType.Mob)
            return;

        mobRef = mob;
    }

    private void SetEmissionPropertyValue(float emission, Renderer renderer)
    {
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_EmissiveStrengh", emission);
        renderer.SetPropertyBlock(propertyBlock);
    }
}