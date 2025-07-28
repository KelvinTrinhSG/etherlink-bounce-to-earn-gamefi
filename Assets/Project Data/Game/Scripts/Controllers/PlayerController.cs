using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    [Header("Settings")]
    public Vector2 platformHitForce;
    public Vector2 jumpPlatfromForce;
    public float tapForceY;
    public float axcelHitCoefX;
    public float axcelHitCoefY;
    public float ralaxModeLinearDrag;
    public float relaxModeSideAxcel;

    [Space(5)]
    public float minJumpDelay = 0.1f;

    [Header("References")]
    public Rigidbody2D rigidbodyRef;
    public SpawnController spawnController;
    public GameController gameController;
    public Animator animatorRef;
    public Transform skinHolder;
    public Transform trailTransform;
    public PowerUpVisualiser powerUpVisualiser;
    public ComboVisualizer comboVisualiser;
    public HitEffectController hitEffectController;
    public GameObject playerApperingParticle;
    public ParticleSystem playerAppearAbsopsionSparcsParticle;
    public ParticleSystem playerAppearCircleParticle;
    public FloatingText comboText1;
    public FloatingText comboText2;
    public FloatingText newComboStateText;
    public Material comboPlatformHihlightMaterial;

    public static Action OnChalangeFinishPassed;
    public static Action OnPlatfromTouched;

    private State state = State.Normal;
    private ComboState comboState;
    private RotationMode rotationMode;

    private Transform transformRef;
    private Transform graphicsTransformRef;
    private Coroutine shieldAnimation;
    private Coroutine doubleJumpAnimation;
    private Coroutine boosterAnimation;
    private Coroutine deathAnimation;
    private ReferenceController referenceController;
    private Pool powerUpsPickUpEffectPool;
    private Pool comboTextPool;
    private Pool spikesBreakPool;

    private AudioClip[] defaultHitSounds;
    private AudioClip portalSound;
    private AudioClip jumpPlatformSound;
    private AudioClip energyFieldSound;
    private AudioClip boosterSound;
    private AudioClip invisiblePlatformSound;
    private AudioClip powerUpPickUp;

    private ParticleSetuper trailParticleSetuper;
    private CameraShake cameraShake;

    private bool taped = false;
    private bool isShieldActive = false;
    private bool isBoosterActive = false;
    private bool isAxcelerometerSupported = true;
    private bool isExtraShieldAllowed = false;
    private bool isTapsDisabled = false;
    private bool isAnimationActive = false;
    private bool isUnicorn = false;

    private bool isFirstTapPerformed = false;
    private bool floatingTextToggle = false;
    private bool usingCustomTrail = false;
    private bool checkForPerfectLanding = false;

    private float lastPlatfomrHitTime;
    private float inertiaCoef = 1f;
    private float spawnSpeedCoef = 1f;
    private float relaxSideAxcelCoef;
    private float horizontalVelBasedRotationY;
    private float shieldAdditionalDuration = 0f;
    private float relaxZoneRotationEulerZ;
    private float prevAxcelY;

    private int updatesAfterHit;
    private int velocityParameter;
    private int onHitParameter;
    private int middleOfScreenInPixels;
    private int combo;
    private int comboPlatformMatColorTintId;

    public static bool IsBoosterActive
    {
        get { return instance.isBoosterActive; }
    }

    public static float SetSpeedCoef
    {
        set { instance.spawnSpeedCoef = value; }
    }

    public static Vector2 Velocity
    {
        get { return instance.rigidbodyRef.velocity; }
    }

    public static State CurrentState
    {
        get { return instance.state; }
    }

    public static ComboState ComboState
    {
        get
        {
            if (instance != null)
            {
                return instance.comboState;
            }
            else
            {
                return ComboState.None;
            }
        }
    }

    public static bool IsTapsDisabled
    {
        get { return instance.isTapsDisabled; }
        set { instance.isTapsDisabled = value; }
    }

    public static Vector3 Position
    {
        get { return instance.transformRef.position; }
    }

    public static void DisableHorizontalAxceleration()
    {
        instance.platformHitForce = instance.platformHitForce.SetX(0f);
    }

    public static float GetPathedDistance()
    {
        return instance.transformRef.position.x;
    }

    private float SpeedCoef
    {
        get { return spawnSpeedCoef * comboSpeedAddition; }
    }

    // achievements fields
    private int bouncesAmount = 0;
    private int powerUpsPicked = 0;

    // combo system fields
    private int basicStateHits = 5;
    private int middleStateHits = 10;
    private int ultraStateHits = 15;

    private float comboSpeedAddition = 1f;
    private float targetComboSpeedAddition = 1f;
    private float basicSpeedAddition = 1.07f;
    private float middleSpeedAddition = 1.2f;
    private float ultraSpeedAddition = 1.5f;

    private Coroutine comboSpeedAdditionChangeCoroutine;

    public enum RotationMode
    {
        Physics,
        Velocity,
    }

    public enum State
    {
        Normal,
        Accelerating,
        HittedJumpPlatform,
    }

    private void Awake()
    {
        transformRef = transform;
        instance = this;

        referenceController = ReferenceController.instance;
        lastPlatfomrHitTime = -1f;
        spawnSpeedCoef = 1f;
        combo = 1;
        isAxcelerometerSupported = SystemInfo.supportsAccelerometer;
        middleOfScreenInPixels = Screen.width / 2;
        isAnimationActive = false;

        velocityParameter = Animator.StringToHash("velocity");
        onHitParameter = Animator.StringToHash("onHit");
        powerUpsPickUpEffectPool = PoolManager.GetPoolByName("PowerUpsPickUp");
        spikesBreakPool = PoolManager.GetPoolByName("SpikesBreakParticle");
        comboPlatformMatColorTintId = Shader.PropertyToID("_TintColor");


        ResetAchievementsResults();
        InitializeSounds();
    }

    private void Start()
    {
        cameraShake = CameraController.instance.cameraShake;
    }

    #region Initialization
    private void InitializeSounds()
    {
        defaultHitSounds = new AudioClip[] { AudioController.Sounds.defaultHitSound };
        jumpPlatformSound = AudioController.Sounds.jumpPlatformSound;
        energyFieldSound = AudioController.Sounds.energyFieldSound;
        boosterSound = AudioController.Sounds.boosterSound;
        invisiblePlatformSound = AudioController.Sounds.invisiblePlatformSound;
        portalSound = AudioController.Sounds.portalSound;
        powerUpPickUp = AudioController.Sounds.powerUpPickUp;
    }

    public void InitiazeSkin(StoreProduct skin, bool startInitialization)
    {
        if (!startInitialization)
        {
            Destroy(graphicsTransformRef.gameObject);
        }

        transformRef.localEulerAngles = Vector3.zero;

        GameObject inst = Instantiate(skin.skinPrefab, skinHolder);
        graphicsTransformRef = inst.transform;
        graphicsTransformRef.localPosition = Vector3.zero;

        rotationMode = skin.rotationMode;
        isUnicorn = skin.skinPrefab.name.Contains("Unicorn");

        if (skin.hitSounds.Length > 0)
        {
            defaultHitSounds = skin.hitSounds;
        }
        else
        {
            defaultHitSounds = new AudioClip[] { AudioController.Sounds.defaultHitSound };
        }

        ParticleSystem hitEffect;

        // if there is a special hit particle
        if (skin.specialHitEffectPrefab != null)
        {
            hitEffect = Instantiate(skin.specialHitEffectPrefab);
        }
        else
        {
            hitEffect = referenceController.defaultHitEffect;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = hitEffect.colorOverLifetime;

            Gradient gradient = new Gradient();
            gradient.SetKeys
                (
                new GradientColorKey[] { new GradientColorKey(skin.hitEffectColor, 0f), new GradientColorKey(skin.hitEffectColor, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
                );

            colorOverLifetimeModule.color = gradient;
        }

        hitEffectController.Init(hitEffect);


        if (!startInitialization)
        {
            if (trailTransform != null)
            {
                trailTransform.SetParent(null);
                trailTransform.gameObject.SetActive(false);
            }
        }

        //removing custom trail if necessary
        if (usingCustomTrail)
        {
            Destroy(trailTransform.gameObject);
        }

        // if there is a special trail particle
        usingCustomTrail = skin.trailPrefab != null;

        if (usingCustomTrail)
        {
            trailTransform = Instantiate(skin.trailPrefab, transformRef).transform;
            trailTransform.localPosition = Vector3.zero;
        }
        else
        {
            trailTransform = referenceController.defaultTrailObject.transform;
            trailTransform.SetParent(transformRef);
            trailTransform.localPosition = Vector3.zero;

            trailTransform.gameObject.SetActive(true);
        }

        trailParticleSetuper = trailTransform.GetComponent<ParticleSetuper>();
        trailParticleSetuper.SetEmissionRate(0f);

        rigidbodyRef.angularVelocity = 0f;
        rigidbodyRef.velocity = new Vector3(0f, prevAxcelY);

        rigidbodyRef.constraints = rotationMode == RotationMode.Velocity ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.None;

        if (!startInitialization)
            return;

        // setuping appearing particle colors

        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModuleRef = playerAppearAbsopsionSparcsParticle.colorOverLifetime;

        Gradient gradientRef = new Gradient();
        gradientRef.SetKeys
            (
            new GradientColorKey[] { new GradientColorKey(skin.hitEffectColor, 0f), new GradientColorKey(skin.hitEffectColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 1f) }
            );

        colorOverLifetimeModuleRef.color = gradientRef;

        colorOverLifetimeModuleRef = playerAppearCircleParticle.colorOverLifetime;

        gradientRef = new Gradient();
        gradientRef.SetKeys
            (
            new GradientColorKey[] { new GradientColorKey(skin.hitEffectColor, 0f), new GradientColorKey(skin.hitEffectColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.2f, 0f), new GradientAlphaKey(0.1f, 0.3f), new GradientAlphaKey(0f, 1f) }
            );

        colorOverLifetimeModuleRef.color = gradientRef;

        isFirstTapPerformed = false;
        RunPlayerAppearAnimation();
    }

    public static void SelectSkin(StoreProduct skin)
    {
        instance.InitiazeSkin(skin, false);
    }
    #endregion

    #region Controlls and Physics
    public void SimulateFirstTap()
    {
        taped = true;
        lastPlatfomrHitTime = -1f;
        isFirstTapPerformed = true;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            OnComboPlatformHitted();
        }
        else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            BreakCombo(false);
        }
#endif

        if (gameController.isGameActive)
        {
            if (Input.GetMouseButtonDown(0) && !PauseManager.PauseState)
            {
                if (!isFirstTapPerformed)
                    return;

                taped = true;
                lastPlatfomrHitTime = -1f;
            }

            if (transformRef.position.y < -2f)
            {
                if (isShieldActive)
                {
                    //Revive(new Vector3(spawnController.SpawnRevivePlatform(), 2f));

                    if (powerUpVisualiser.ShieldTimeLeft < 2f)
                    {
                        shieldAdditionalDuration = 2f - powerUpVisualiser.ShieldTimeLeft;
                        powerUpVisualiser.ShieldTimeLeft = 2f;
                    }

                    transformRef.position = transformRef.position.AddToX(1f).SetY(3f);
                }
                else
                {
                    BreakCombo(false);
                    OnPlayerDead(DeathReason.Falling);
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space) && !PauseManager.PauseState)
            {
                if (!isFirstTapPerformed)
                    return;

                taped = true;
                lastPlatfomrHitTime = -1f;
            }
#endif
        }
        else
        {
            if (transformRef.position.y < -4f)
            {
                DisablePlayer();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!gameController.isGameActive)
        {
            prevAxcelY = rigidbodyRef.velocity.y;

            if (!isFirstTapPerformed && Mathf.Abs(transformRef.position.x) > 0.1)
            {
                rigidbodyRef.MovePosition(new Vector2(0f, Mathf.Clamp(transformRef.position.y, 0f, 5f)));
            }

            return;
        }

        if (taped && updatesAfterHit >= 1 && (state == State.Normal || state == State.HittedJumpPlatform) && !isTapsDisabled)
        {
            taped = false;

            if (isBoosterActive)
            {
                BreakBooster();
            }

            rigidbodyRef.AddForce(Vector2.zero.SetY(-rigidbodyRef.velocity.y - tapForceY * (state == State.HittedJumpPlatform ? 1.5f : 1f) * SpeedCoef), ForceMode2D.Impulse);

            if (state == State.Normal)
            {
                inertiaCoef = (axcelHitCoefX - 1f) * 0.5f + 1f;
            }

            state = State.Accelerating;
        }
        else if (updatesAfterHit < 1)
        {
            updatesAfterHit++;
        }
        else
        {
            taped = false;
        }


        if (rotationMode == RotationMode.Velocity)
        {
            // animation is not active while player is appearing
            if (isAnimationActive)
            {
                animatorRef.SetFloat(velocityParameter, rigidbodyRef.velocity.y);
            }

            // velocity rotation 
            if (rigidbodyRef.velocity != Vector2.zero)
            {
                Vector3 vecrticalVelBasedRotation = Vector3.zero.SetX(-5).SetY(Mathf.Clamp(rigidbodyRef.velocity.y * 0.25f, -0.5f, 1f));
                float horizontalVelBasedRotationYAddition = Mathf.Clamp(rigidbodyRef.velocity.x * 0.33f - 1.5f, 0f, 1.2f);

                if (rigidbodyRef.velocity.y < 0)
                {
                    horizontalVelBasedRotationYAddition *= Mathf.Clamp(1 + rigidbodyRef.velocity.y * 0.2f, 0f, 1f);
                }

                graphicsTransformRef.rotation = Quaternion.LookRotation(vecrticalVelBasedRotation.AddToY(horizontalVelBasedRotationYAddition));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (lastPlatfomrHitTime + minJumpDelay > Time.timeSinceLevelLoad || transformRef.position.y < collision.gameObject.transform.position.y - 1f)
            {
                return;
            }

            lastPlatfomrHitTime = Time.timeSinceLevelLoad;
            Platform hittedPlatform = collision.gameObject.GetComponent<Platform>();

            if (combo >= middleStateHits && hittedPlatform.type != PlatformType.Jump)
            {
                hittedPlatform.RunComboAnimation(comboState);
            }

            // if ball is lower than allowed or hitted left corner
            if (transformRef.position.y < -2f || collision.collider.bounds.min.x - transformRef.position.x > 0.25f)
            {
                AudioController.PlaySound(defaultHitSounds.GetRandomItem(), 1);

                //Debug.Log("Platform hitted left corner | transform pos: " + transformRef.position + "  collider min x: " + collision.collider.bounds.min.x + "  is combo: " + hittedPlatform.isComboPlatform);
                rigidbodyRef.AddForce(new Vector2(-0.2f - rigidbodyRef.velocity.x, -0.3f), ForceMode2D.Impulse);

                if (hittedPlatform.isComboPlatform)
                {
                    OnComboPlatformHitted();
                }
                else
                {
                    OnNotComboPlatformHitted();
                }

                return;
            }
            // if ball hitted right corner
            else if (transformRef.position.x - collision.collider.bounds.max.x > 0.3f)
            {
                AudioController.PlaySound(defaultHitSounds.GetRandomItem(), 1);

                if (transformRef.position.x - collision.collider.bounds.max.x < 0.45f)
                {
                    //Debug.Log("hitted right corner | transform pos: " + transformRef.position + "  collider max x: " + collision.collider.bounds.max.x);
                    rigidbodyRef.AddForce(new Vector2(platformHitForce.x * inertiaCoef * SpeedCoef * 1.3f * (state == State.Accelerating ? axcelHitCoefY : 1) - rigidbodyRef.velocity.x, platformHitForce.y * (state == State.Accelerating ? axcelHitCoefY : 1) - rigidbodyRef.velocity.y), ForceMode2D.Impulse);
                    state = State.Normal;

                    if (hittedPlatform.isComboPlatform)
                    {
                        OnComboPlatformHitted();
                        hittedPlatform.DisableComboLook(true);
                    }
                    else
                    {
                        OnNotComboPlatformHitted();
                    }
                }
                else
                {
                    //Debug.Log("hitted right corner !!TOO FAR!! | transform pos: " + transformRef.position + "  collider max x: " + collision.collider.bounds.max.x);
                    rigidbodyRef.AddForce(new Vector2(platformHitForce.x * inertiaCoef * SpeedCoef * (state == State.Accelerating ? axcelHitCoefY : 1) - rigidbodyRef.velocity.x, rigidbodyRef.velocity.y * 0.5f), ForceMode2D.Impulse);
                }
                return;
            }

            // playing sound
            if (hittedPlatform.type == PlatformType.Jump)
            {
                //AudioController.PlaySound(jumpPlatformSound);
                AudioController.PlaySound(jumpPlatformSound, 1);

#if MODULE_VIBRATION
                if (GameSettingsPrefs.Get<bool>("vibration"))
                {
                    if (AudioController.Settings.vibrations.shortVibration != 0)
                    {
                        Vibration.Vibrate(AudioController.Settings.vibrations.shortVibration);
                    }
                }
#endif
            }
            else if (hittedPlatform.type == PlatformType.Invisible)
            {
                AudioController.PlaySound(invisiblePlatformSound);
            }
            else
            {
                AudioController.PlaySound(defaultHitSounds.GetRandomItem());
            }

            if (rotationMode == RotationMode.Velocity)
            {
                animatorRef.SetTrigger(onHitParameter);
            }

            if (hittedPlatform.type == PlatformType.Jump)
            {
                state = State.HittedJumpPlatform;
            }
            else if (hittedPlatform.type == PlatformType.Spiked)
            {
                if (isShieldActive || combo >= middleStateHits)
                {
                    collision.gameObject.GetComponent<Platform>().DestroySpikes();
                    cameraShake.ShakeOnce(2.5f, 3.5f, 0.1f, 0.3f);
                }
                else
                {
                    OnPlayerDead(DeathReason.Spikes);
                    state = State.Normal;
                }

                BreakCombo();
            }
            else if (state != State.Accelerating)
            {
                state = State.Normal;
            }

            if (!ComboTrigger.IsHittedComboPlatform())
            {
                OnNotComboPlatformHitted();
            }

            if (!isBoosterActive)
            {
                if (state == State.Normal)
                {
                    rigidbodyRef.AddForce(new Vector2(platformHitForce.x * inertiaCoef * SpeedCoef * (isFirstTapPerformed ? 1f : 0f) - rigidbodyRef.velocity.x, platformHitForce.y - rigidbodyRef.velocity.y), ForceMode2D.Impulse);

                    inertiaCoef = 1f;
                }
                else if (state == State.Accelerating)
                {
                    rigidbodyRef.AddForce(new Vector2(platformHitForce.x * axcelHitCoefX * SpeedCoef - rigidbodyRef.velocity.x, platformHitForce.y * axcelHitCoefY - rigidbodyRef.velocity.y), ForceMode2D.Impulse);
                    state = State.Normal;
                }
                // hitted jump platform
                else
                {
                    rigidbodyRef.AddForce(new Vector2(jumpPlatfromForce.x * inertiaCoef * SpeedCoef - rigidbodyRef.velocity.x, jumpPlatfromForce.y - rigidbodyRef.velocity.y), ForceMode2D.Impulse);

                    inertiaCoef += (jumpPlatfromForce.x / platformHitForce.x - 1f) * 0.1f; // adding additional 10 percents to inertia axceleration
                    if (inertiaCoef > 2f)
                    {
                        inertiaCoef = 2f;
                    }
                }
            }

            updatesAfterHit = 0;

            hitEffectController.Play(transformRef.position.SetY(hittedPlatform.transform.position.y + 0.02f));

            bouncesAmount++;
            isExtraShieldAllowed = false;
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (rigidbodyRef.velocity.y == 0 && !isBoosterActive)
        {
            rigidbodyRef.AddForce(new Vector2(platformHitForce.x * axcelHitCoefX * SpeedCoef - rigidbodyRef.velocity.x, platformHitForce.y * axcelHitCoefY - rigidbodyRef.velocity.y), ForceMode2D.Impulse);

            state = State.Normal;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ShieldPlatform") && (isShieldActive || isExtraShieldAllowed || comboState == ComboState.Ultra))
        {
            if (lastPlatfomrHitTime + minJumpDelay > Time.timeSinceLevelLoad)
            {
                return;
            }

            if (!isShieldActive && !isExtraShieldAllowed && comboState == ComboState.Ultra)
            {
                // if player is clother to current platform than to any other applying effect fully
                if (Mathf.Abs(transformRef.position.x - collision.gameObject.transform.position.x) < 0.5f)
                {
                    BreakCombo();
                }
                // otherwise absolutely ignoring
                else
                {
                    return;
                }
            }

            isExtraShieldAllowed = false;
            lastPlatfomrHitTime = Time.timeSinceLevelLoad;

            AudioController.PlaySound(invisiblePlatformSound);

            state = State.HittedJumpPlatform;
            rigidbodyRef.AddForce(new Vector2(platformHitForce.x * inertiaCoef * axcelHitCoefX * SpeedCoef - rigidbodyRef.velocity.x, platformHitForce.y * axcelHitCoefY - rigidbodyRef.velocity.y), ForceMode2D.Impulse);
            inertiaCoef = 1f;

            collision.gameObject.GetComponent<ShieldPlaformBehaviour>().OnHit();
        }
        else if (collision.gameObject.CompareTag("LevelBorder"))
        {
            ColorsController.SetNextLevelPreset(collision.GetComponent<LevelBorderBehaviour>().PreferedColorsPreset);
        }
        else if (collision.gameObject.CompareTag("Mob"))
        {
            if (isShieldActive || combo >= middleStateHits)
            {
                collision.gameObject.SetActive(false);
                spikesBreakPool.GetPooledObject(collision.gameObject.transform.position);
                cameraShake.ShakeOnce(2.5f, 3.5f, 0.1f, 0.3f);

#if MODULE_VIBRATION
                if (GameSettingsPrefs.Get<bool>("vibration"))
                {
                    if (AudioController.Settings.vibrations.shortVibration != 0)
                    {
                        Vibration.Vibrate(AudioController.Settings.vibrations.shortVibration);
                    }
                }
#endif
            }
            else
            {
                state = State.Normal;
                inertiaCoef = 1f;
                OnPlayerDead(DeathReason.Mob);
            }

            BreakCombo();
        }
        else if (collision.gameObject.CompareTag("PowerUpDisabler"))
        {
            BreakAllPowerUps();
        }
        else if (collision.gameObject.CompareTag("ScorePointer"))
        {
            gameController.RunHighscorePointerAnimation();
        }
        else if (!isBoosterActive)
        {
            if (collision.gameObject.CompareTag("EnergyField"))
            {
                AudioController.PlaySound(energyFieldSound);

                collision.gameObject.GetComponent<EnergyFieldBehaviour>().OnPickedUp();

                state = State.Normal;
                rigidbodyRef.AddForce(new Vector2(PowerUp.energyFieldForce.x * inertiaCoef * SpeedCoef - rigidbodyRef.velocity.x, PowerUp.energyFieldForce.y - rigidbodyRef.velocity.y), ForceMode2D.Impulse);

                powerUpsPicked++;
            }
            else if (collision.gameObject.CompareTag("Shield"))
            {
                AudioController.PlaySound(powerUpPickUp);

                collision.gameObject.SetActive(false);
                powerUpsPickUpEffectPool.GetPooledObject(collision.gameObject.transform.position);
                InitializeShield();
            }
            else if (collision.gameObject.CompareTag("Booster"))
            {
                isBoosterActive = true;

                AudioController.PlaySound(powerUpPickUp);

                collision.gameObject.SetActive(false);
                powerUpsPickUpEffectPool.GetPooledObject(collision.gameObject.transform.position);
                InitializeBooster();
            }
            else if (collision.gameObject.CompareTag("Portal"))
            {
                AudioController.PlaySound(portalSound);

                rigidbodyRef.simulated = false;
                graphicsTransformRef.gameObject.SetActive(false);
                trailTransform.gameObject.SetActive(false);

                state = State.Normal;
                transformRef.DOMove(collision.gameObject.GetComponent<Portal>().otherSidePortal.transform.position, 0.5f).OnComplete(() =>
                {
                    StartCoroutine(OnPortalExit());
                });

                powerUpVisualiser.HideGraphics(0.5f);

                AchievementManager.IncrementProgress(18, 1); //Pass through 10 portals
            }
        }
    }
    #endregion

    #region Animations
    private IEnumerator OnPortalExit()
    {
        yield return new WaitForFixedUpdate();

        graphicsTransformRef.gameObject.SetActive(true);
        trailTransform.gameObject.SetActive(true);
        comboVisualiser.ShowGraphics();

        rigidbodyRef.simulated = true;

        rigidbodyRef.velocity = Vector2.zero;
        rigidbodyRef.AddForce(new Vector2(0f, -15f), ForceMode2D.Impulse);
    }

    public void Revive(Vector3 respawnPostion)
    {
        transform.DOMove(respawnPostion, 0.8f).OnComplete(() =>
        {
            EnablePlayer();
            gameController.OnPlayerRevived();

            state = State.Normal;
            inertiaCoef = 1;

            rigidbodyRef.AddForce(new Vector2(-rigidbodyRef.velocity.x, -rigidbodyRef.velocity.y), ForceMode2D.Impulse);

            if (deathAnimation != null)
            {
                StopCoroutine(deathAnimation);
            }

            graphicsTransformRef.gameObject.layer = LayerMask.NameToLayer("Player");
            graphicsTransformRef.localPosition = Vector3.zero;

            InitializeShield(3f);
        });
    }

    private void RunPlayerAppearAnimation()
    {
        StartCoroutine(PlayerApperingCoroutine());
    }

    private IEnumerator PlayerApperingCoroutine()
    {
        DisablePlayer();

        playerApperingParticle.transform.position = transformRef.position;
        playerApperingParticle.SetActive(true);

        yield return new WaitForSeconds(1f);

        EnablePlayer();
        Tween.DelayedCall(0.5f, () => playerApperingParticle.SetActive(false));
        transformRef.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < 0.15f)
        {
            transformRef.localScale = Vector3.zero.SetAll(Mathf.Lerp(0f, 1.4f, timer / 0.15f));
            timer += Time.deltaTime;

            if (taped)
            {
                transformRef.localScale = Vector3.one;
                yield break;
            }

            yield return null;
        }

        timer = 0.15f;
        while (timer > 0)
        {
            transformRef.localScale = Vector3.zero.SetAll(Mathf.Lerp(1f, 1.4f, timer / 0.15f));
            timer -= Time.deltaTime;

            if (taped)
            {
                transformRef.localScale = Vector3.one;
                yield break;
            }

            yield return null;
        }

        transformRef.localScale = Vector3.one;
        isAnimationActive = true;
    }

    private void DisablePlayer()
    {
        rigidbodyRef.isKinematic = true;
        rigidbodyRef.simulated = false;
        animatorRef.gameObject.SetActive(false);
        trailTransform.gameObject.SetActive(false);
    }

    private void EnablePlayer()
    {
        rigidbodyRef.isKinematic = false;
        rigidbodyRef.simulated = true;

        animatorRef.gameObject.SetActive(true);
        trailTransform.gameObject.SetActive(true);
    }

    private void OnPlayerDead(DeathReason deathReason)
    {
        if (deathReason == DeathReason.Spikes)
        {
            AchievementManager.UnlockAchievement(19); //Hit spikes
        }

        gameController.GameOver(deathReason);

        if (deathReason != DeathReason.Falling)
        {
            //DisablePlayer();
            deathAnimation = StartCoroutine(DeathAnimationCoroutine());
        }

#if MODULE_VIBRATION
        if (GameSettingsPrefs.Get<bool>("vibration"))
        {
            if (AudioController.Settings.vibrations.longVibration != 0)
            {
                Vibration.Vibrate(AudioController.Settings.vibrations.longVibration);
            }
        }
#endif
    }

    public static void PlayDeathAnimation()
    {
        instance.StartCoroutine(instance.DeathAnimationCoroutine());
    }

    private IEnumerator DeathAnimationCoroutine()
    {
        rigidbodyRef.AddForce(rigidbodyRef.velocity * -1, ForceMode2D.Impulse);
        rigidbodyRef.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);

        graphicsTransformRef.gameObject.layer = 0;

        float timer = 0;
        while (timer < 1)
        {
            graphicsTransformRef.position = graphicsTransformRef.position.SetZ(timer);
            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Power Ups
    private void InitializeShield(float overrideDuration = -1)
    {
        if (shieldAnimation != null)
        {
            StopCoroutine(shieldAnimation);
        }

        shieldAnimation = StartCoroutine(ShieldAnimation(overrideDuration == -1 ? PowerUp.shieldDuration : overrideDuration));
        powerUpsPicked++;
    }

    private IEnumerator ShieldAnimation(float shieldDuration)
    {
        powerUpVisualiser.StartVisualisation(PowerUpType.Shield, shieldDuration);
        isShieldActive = true;

        yield return new WaitForSeconds(shieldDuration);

        if (shieldAdditionalDuration > 0)
        {
            yield return new WaitForSeconds(shieldAdditionalDuration);
        }

        isShieldActive = false;
    }

    private void BreakShield()
    {
        isShieldActive = false;

        if (lastPlatfomrHitTime + 0.2f > Time.timeSinceLevelLoad)
        {
            isExtraShieldAllowed = true;
        }
    }

    private void InitializeBooster()
    {
        state = State.HittedJumpPlatform;
        inertiaCoef = 1.5f;

        StartCoroutine(BoosterTrajectoryController());

        checkForPerfectLanding = true;
        powerUpsPicked++;

        BoosterFloatLightBehaviour.instance.Activate();
    }

    private IEnumerator BoosterTrajectoryController()
    {
        isBoosterActive = true;
        float boosterDuration = PowerUp.boosterDuration;
        float timer = 0;
        float verticalVelosity = 2f;
        float horizontalVelocity = 20f;

        float gravityCoef = verticalVelosity * 1.5f * Time.fixedDeltaTime / boosterDuration;
        float airResistanceCoef = -horizontalVelocity * 1.1f * Time.fixedDeltaTime / boosterDuration;

        powerUpVisualiser.StartVisualisation(PowerUpType.Booster, boosterDuration);
        graphicsTransformRef.gameObject.layer = LayerMask.NameToLayer("UI"); // this is done to avoid unnecessary collisions during booster using

        rigidbodyRef.velocity = Vector2.zero;
        rigidbodyRef.gravityScale = 0;

        WaitForFixedUpdate delay = new WaitForFixedUpdate();
        yield return delay;

        rigidbodyRef.simulated = true;
        rigidbodyRef.AddForce(new Vector2(horizontalVelocity, verticalVelosity), ForceMode2D.Impulse);

        while (isBoosterActive && timer < boosterDuration)
        {
            rigidbodyRef.velocity = rigidbodyRef.velocity.AddToY(-gravityCoef);

            if (timer > boosterDuration * 0.7f)
            {
                rigidbodyRef.velocity = rigidbodyRef.velocity.AddToX(airResistanceCoef);
            }

            timer += Time.fixedDeltaTime;
            yield return delay;
        }

        graphicsTransformRef.gameObject.layer = LayerMask.NameToLayer("Player");

        isBoosterActive = false;
        rigidbodyRef.gravityScale = 1;
    }

    private void BreakBooster()
    {
        graphicsTransformRef.gameObject.layer = LayerMask.NameToLayer("Player");

        isBoosterActive = false;
        rigidbodyRef.gravityScale = 1;

        powerUpVisualiser.BreakBoosterAnimation();
    }

    private void BreakAllPowerUps()
    {
        BreakBooster();
        BreakShield();
    }
    #endregion

    #region Achievements

    public void SendAchievementsResults()
    {
        AchievementManager.IncrementProgress(3, bouncesAmount); //Jump 500 times
        AchievementManager.IncrementProgress(4, bouncesAmount); //Jump 2000 times

        AchievementManager.IncrementProgress(10, powerUpsPicked); //Pick up 5 power ups
        AchievementManager.IncrementProgress(11, powerUpsPicked); //Pick up 20 power ups

        ResetAchievementsResults();
    }

    private void ResetAchievementsResults()
    {
        bouncesAmount = 0;
        powerUpsPicked = 0;
    }

    #endregion

    #region Combo

    public static void OnComboPlatformHitted()
    {
        if (instance.checkForPerfectLanding && instance.combo < instance.ultraStateHits)
        {
            instance.combo = instance.ultraStateHits;

        }
        else
        {
            instance.combo++;
        }

        instance.gameController.AddScorePoints(instance.combo);
        instance.OnComboIncreased();
    }

    private void OnNotComboPlatformHitted()
    {
        if (combo != 1)
        {
            BreakCombo();
        }

        checkForPerfectLanding = false;
        gameController.AddScorePoints(1);
    }

    private void OnComboIncreased()
    {
        if (combo == 1)
        {
            InitNoneComboState();
            return;
        }
        else if (combo == basicStateHits)
        {
            InitBasicComboState();
            return;
        }
        else if (combo == middleStateHits)
        {
            InitMiddleComboState();
            return;
        }
        else if (combo == ultraStateHits)
        {
            InitUltraComboState();
            return;
        }
        else
        {
            DisplayComboText("COMBO X" + combo, FloatingText.TextStyle.Normal);
        }

        AchievementManager.SetProgress(7, combo); //Reach 5 combos
        AchievementManager.SetProgress(8, combo); //Reach 10 combos
        AchievementManager.SetProgress(9, combo); //Reach 15 combos
    }

    private void InitNoneComboState()
    {
        comboState = ComboState.None;
        ChangeComboSpeedAddition(1f);

        comboVisualiser.OnNewState(comboState);
        trailParticleSetuper.SetEmissionRate(0f);

        comboPlatformHihlightMaterial.SetColor(comboPlatformMatColorTintId, ComboVisualizer.GetStateColor(comboState).SetAlpha(0.1f));
    }

    private void InitBasicComboState()
    {
        comboState = ComboState.Basic;
        ChangeComboSpeedAddition(basicSpeedAddition);
        DisplayNewComboStateText("AMAZING!\nX" + combo, FloatingText.TextStyle.BasicState/*, () => cameraShake.ShakeOnce(3f, 1.5f, 0.1f, 0.2f)*/);

        comboVisualiser.OnNewState(comboState);
        trailParticleSetuper.SetEmissionRate(10f);
        if (!usingCustomTrail)
        {
            trailParticleSetuper.SetColor(comboVisualiser.basicStateColor);
        }

        comboPlatformHihlightMaterial.SetColor(comboPlatformMatColorTintId, ComboVisualizer.GetStateColor(comboState).SetAlpha(0.1f));

#if MODULE_VIBRATION
        if (GameSettingsPrefs.Get<bool>("vibration"))
        {
            if (AudioController.Settings.vibrations.shortVibration != 0)
            {
                Vibration.Vibrate(AudioController.Settings.vibrations.shortVibration);
            }
        }
#endif
    }

    private void InitMiddleComboState()
    {
        comboState = ComboState.Middle;
        ChangeComboSpeedAddition(middleSpeedAddition);
        DisplayNewComboStateText("FEVER!\nX" + combo, FloatingText.TextStyle.MiddleState/*, () => cameraShake.ShakeOnce(5f, 1.5f, 0.1f, 0.2f)*/);

        comboVisualiser.OnNewState(comboState);
        trailParticleSetuper.SetEmissionRate(25f);
        if (!usingCustomTrail)
        {
            trailParticleSetuper.SetColor(comboVisualiser.middleStateColor);
        }

        comboPlatformHihlightMaterial.SetColor(comboPlatformMatColorTintId, ComboVisualizer.GetStateColor(comboState).SetAlpha(0.1f));

#if MODULE_VIBRATION
        if (GameSettingsPrefs.Get<bool>("vibration"))
        {
            if (AudioController.Settings.vibrations.shortVibration != 0)
            {
                Vibration.Vibrate(AudioController.Settings.vibrations.shortVibration);
            }
        }
#endif
    }

    private void InitUltraComboState()
    {
        comboState = ComboState.Ultra;
        ChangeComboSpeedAddition(ultraSpeedAddition);
        DisplayNewComboStateText((checkForPerfectLanding ? "PERFECT LANDING!\nX" : "ULTRA FEVER!\nX") + combo, FloatingText.TextStyle.UltraState/*, () => cameraShake.ShakeOnce(8f, 1.5f, 0.1f, 0.2f)*/);

        ColorsController.SetUltraPreset();
        comboVisualiser.OnNewState(comboState);
        trailParticleSetuper.SetEmissionRate(50f);
        if (!usingCustomTrail)
        {
            trailParticleSetuper.SetColor(comboVisualiser.ultraStateColor);
        }

        comboPlatformHihlightMaterial.SetColor(comboPlatformMatColorTintId, ComboVisualizer.GetStateColor(comboState).SetAlpha(0.1f));

#if MODULE_VIBRATION
        if (GameSettingsPrefs.Get<bool>("vibration"))
        {
            if (AudioController.Settings.vibrations.shortVibration != 0)
            {
                Vibration.Vibrate(AudioController.Settings.vibrations.shortVibration);
            }
        }
#endif
    }

    private void BreakCombo(bool showUI = true)
    {
        if (comboState == ComboState.Ultra)
        {
            ColorsController.OnUltraComboBroke();
        }

        combo = 1;
        OnComboIncreased();

        if (showUI)
        {
            DisplayComboText("MISS", FloatingText.TextStyle.ComboBreak);
        }
    }

    private void DisplayComboText(string text, FloatingText.TextStyle style)
    {
        if (floatingTextToggle)
        {
            comboText2.AxcelerateFade();
            comboText1.Init(text, style);
        }
        else
        {
            comboText1.AxcelerateFade();
            comboText2.Init(text, style);
        }

        floatingTextToggle = !floatingTextToggle;
    }

    private void DisplayNewComboStateText(string text, FloatingText.TextStyle style/*, Action onApperedAction*/)
    {
        comboText1.AxcelerateFade();
        comboText2.AxcelerateFade();
        newComboStateText.Init(text, style/*, onTextAppeared: onApperedAction*/);
    }

    private void ChangeComboSpeedAddition(float newValue)
    {
        if (comboSpeedAdditionChangeCoroutine != null)
        {
            StopCoroutine(comboSpeedAdditionChangeCoroutine);
        }

        comboSpeedAdditionChangeCoroutine = StartCoroutine(ChangeSpeedAddition(newValue));
    }

    private IEnumerator ChangeSpeedAddition(float target)
    {
        targetComboSpeedAddition = target;
        float delta = targetComboSpeedAddition - comboSpeedAddition;
        float changeSpeed = 0.2f;
        float sign = Mathf.Sign(delta);

        while (Mathf.Abs(delta) > 0.01f)
        {
            comboSpeedAddition += changeSpeed * sign * Time.deltaTime;
            delta = targetComboSpeedAddition - comboSpeedAddition;

            yield return null;
        }
    }


    #endregion
}