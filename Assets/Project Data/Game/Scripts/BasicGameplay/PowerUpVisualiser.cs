using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class PowerUpVisualiser : MonoBehaviour
{
    public GameObject shieldAnimationObject;
    public GameObject boosterAnimationObject;
    public GameObject coinsMagnetAnimationObject;

    private Transform playerTransform;
    private Coroutine shieldAnimationCoroutine;
    private Coroutine boosterAnimationCoroutine;
    private Coroutine coinsMagnetAnimationCoroutine;
    private Coroutine hideGraphicsCoroutine;

    private bool active = false;
    private bool shieldActive = false;
    private bool boosterActive = false;
    private bool coinsMagnetActive = false;

    private float shieldTimer = 0;

    public float ShieldTimeLeft
    {
        get { return shieldTimer; }
        set { shieldTimer = value; }
    }

    private void Awake()
    {
        playerTransform = ReferenceController.instance.playerController.transform;
    }

    private void Start()
    {
        Tween.DelayedCall(0.5f, () =>
        {
            shieldAnimationObject.SetActive(false);
            boosterAnimationObject.SetActive(false);
            coinsMagnetAnimationObject.SetActive(false);

            gameObject.SetActive(false);
        });
    }

    public void StartVisualisation(PowerUpType powerUpType, float visualisationDuration)
    {
        gameObject.SetActive(true);
        active = true;

        if (powerUpType == PowerUpType.Shield)
        {
            if (shieldAnimationCoroutine != null)
            {
                StopCoroutine(shieldAnimationCoroutine);
            }

            shieldAnimationCoroutine = StartCoroutine(ShieldAnimation(visualisationDuration));
        }
        else if (powerUpType == PowerUpType.Booster)
        {
            if (boosterAnimationCoroutine != null)
            {
                StopCoroutine(boosterAnimationCoroutine);
            }

            boosterAnimationCoroutine = StartCoroutine(BoosterAnimation(visualisationDuration));
        }
        else
        {
            if (coinsMagnetAnimationCoroutine != null)
            {
                StopCoroutine(coinsMagnetAnimationCoroutine);
            }

            coinsMagnetAnimationCoroutine = StartCoroutine(CoinsMagnetAnimation(visualisationDuration));
        }
    }

    private IEnumerator ShieldAnimation(float shieldDuration)
    {
        shieldActive = true;
        shieldAnimationObject.SetActive(true);

        shieldTimer = shieldDuration;

        while (shieldTimer > 0.8f)
        {
            shieldTimer -= Time.deltaTime;
            yield return null;
        }

        while (shieldTimer > 0.2)
        {
            shieldAnimationObject.SetActive(!shieldAnimationObject.activeSelf);

            shieldTimer -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        shieldAnimationObject.SetActive(false);

        while (shieldTimer > 0)
        {
            shieldTimer -= Time.deltaTime;
            yield return null;
        }

        shieldActive = false;
        Deactivate();
    }

    private IEnumerator BoosterAnimation(float boosterDuration)
    {
        boosterActive = true;
        boosterAnimationObject.SetActive(true);

        float timer = boosterDuration;

        while (timer > 1f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        boosterAnimationObject.SetActive(false);
        boosterActive = false;
        Deactivate();
    }

    private IEnumerator CoinsMagnetAnimation(float coinsMagnetDuration)
    {
        coinsMagnetActive = true;
        coinsMagnetAnimationObject.SetActive(true);

        float timer = coinsMagnetDuration;

        while (timer > 1f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        while (timer > 0)
        {
            coinsMagnetAnimationObject.SetActive(!coinsMagnetAnimationObject.activeSelf);

            timer -= 0.15f;
            yield return new WaitForSeconds(0.1f);
        }

        coinsMagnetAnimationObject.SetActive(false);
        coinsMagnetActive = false;
        Deactivate();
    }

    public void HideGraphics(float duration)
    {
        if (active)
        {
            boosterAnimationObject.transform.localPosition = Vector3.zero.SetZ(-20);
            shieldAnimationObject.transform.localPosition = Vector3.zero.SetZ(-20);
            coinsMagnetAnimationObject.transform.localPosition = Vector3.zero.SetZ(-20);

            hideGraphicsCoroutine = StartCoroutine(HideGraphicsCoroutine(duration));
        }
    }

    private IEnumerator HideGraphicsCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        EnableGraphics();
    }

    private void EnableGraphics()
    {
        boosterAnimationObject.transform.localPosition = Vector3.zero;
        shieldAnimationObject.transform.localPosition = Vector3.zero;
        coinsMagnetAnimationObject.transform.localPosition = Vector3.zero;
    }

    public void BreakBoosterAnimation()
    {
        if (boosterActive)
        {
            StopCoroutine(boosterAnimationCoroutine);
            boosterAnimationObject.SetActive(false);
            boosterActive = false;
        }

        Deactivate();
    }

    public void BreakShieldAnimation()
    {
        if (shieldActive)
        {
            StopCoroutine(shieldAnimationCoroutine);
            shieldAnimationObject.SetActive(false);
            shieldActive = false;
        }

        Deactivate();
    }

    public void BreakCoinsMagnetAnimation()
    {
        if (coinsMagnetActive)
        {
            StopCoroutine(coinsMagnetAnimationCoroutine);
            coinsMagnetAnimationObject.SetActive(false);
            coinsMagnetActive = false;
        }

        Deactivate();
    }

    private void Deactivate()
    {
        if (!boosterActive && !shieldActive && !coinsMagnetActive)
        {
            if (hideGraphicsCoroutine != null)
            {
                StopCoroutine(hideGraphicsCoroutine);
                EnableGraphics();
            }

            active = false;
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (active)
        {
            transform.position = playerTransform.position;
        }
    }
}