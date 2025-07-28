using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class InvisiblePlatformBehaviour : Platform
{
    [Header("References")]
    public Material invisiblePlMaterial;
    public Material normalPlMaterial;
    public Collider2D colliderRef;

    private List<Renderer> allRenderers = new List<Renderer>();
    private Coroutine animationActivatorCoroutine;

    private bool isAnimationAvailable = true;


    protected override void OnEnable()
    {
        base.OnEnable();
        isAnimationAvailable = true;

        allRenderers.Clear();
        //allRenderers.Add(meshRenderer);

        //for (int i = 0; i < allRenderers.Count; i++)
        //{
        //    SetAlphaPropertyValue(0.2f, allRenderers[i]);
        //}
    }

    public override void AddPlatformPart(GameObject platformPart, bool isSpikedPlatformSpawnedNow)
    {
        base.AddPlatformPart(platformPart, isSpikedPlatformSpawnedNow);
        allRenderers.Add(platformPart.GetComponent<InvisiblePlatformBehaviour>().meshRenderer);
        SetAlphaPropertyValue(0.2f, allRenderers[allRenderers.Count - 1]);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAnimationAvailable)
        {
            int startPlatformIndex = (int)(PlayerController.Position.x - colliderRef.bounds.min.x);
            StartCoroutine(ColorAnimationController(startPlatformIndex));

            isAnimationAvailable = false;
        }
    }

    private IEnumerator ColorAnimationController(int animationStartIndex)
    {
        animationStartIndex = Mathf.Clamp(animationStartIndex, 0, allRenderers.Count - 1);

        StartCoroutine(PlatformColorAnimation(allRenderers[animationStartIndex]));

        int leftIndex = animationStartIndex - 1;
        int rightIndex = animationStartIndex + 1;

        float delay = 0.06f;

        while (leftIndex >= 0 || rightIndex < allRenderers.Count)
        {
            yield return new WaitForSeconds(delay);
            delay *= 0.7f;

            if (leftIndex >= 0)
            {
                StartCoroutine(PlatformColorAnimation(allRenderers[leftIndex]));
                leftIndex--;
            }

            if (rightIndex < allRenderers.Count)
            {
                StartCoroutine(PlatformColorAnimation(allRenderers[rightIndex]));
                rightIndex++;
            }
        }
    }

    private IEnumerator PlatformColorAnimation(Renderer renderer)
    {
        SetAlphaPropertyValue(0.2f, renderer);

        yield return null;

        Tween.DoFloat(0.2f, 1f, 0.2f, (float alpha) => SetAlphaPropertyValue(alpha, renderer)).SetEasing(Ease.Type.CubicOut);

        yield return new WaitForSeconds(0.3f);

        Tween.DoFloat(1f, 0.2f, 0.45f, (float alpha) => SetAlphaPropertyValue(alpha, renderer));
    }

    private void SetAlphaPropertyValue(float alphaValue, Renderer renderer)
    {
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_Alpha", alphaValue);
        renderer.SetPropertyBlock(propertyBlock);
    }
}