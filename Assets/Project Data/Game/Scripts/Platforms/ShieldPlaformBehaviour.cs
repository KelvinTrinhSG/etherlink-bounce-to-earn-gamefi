using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPlaformBehaviour : Platform
{
    public Renderer graphicsRenderer;
    private Coroutine fadeCoroutine;

    private float maxAlphaValue = 0.7f;

    protected override void Awake()
    {
        //base.Awake();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void OnHit()
    {
        graphicsRenderer.enabled = true;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(ColorFade(0.5f));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetAlphaPropertyValue(0);
    }

    private IEnumerator ColorFade(float fadeTime)
    {
        SetAlphaPropertyValue(maxAlphaValue);

        yield return null;

        float timer = fadeTime;
        while (timer > 0)
        {
            SetAlphaPropertyValue(timer / fadeTime * maxAlphaValue);

            timer -= Time.deltaTime;

            yield return null;
        }

        SetAlphaPropertyValue(0);
    }

    private void SetAlphaPropertyValue(float alphaValue)
    {
        graphicsRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_Alpha", alphaValue);
        graphicsRenderer.SetPropertyBlock(propertyBlock);
    }
}