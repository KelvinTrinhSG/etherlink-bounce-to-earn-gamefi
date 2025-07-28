using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class ComboTrigger : MonoBehaviour
{
    public GameObject platformHighlight;

    private Platform platform;
    private Pool platformHitEffectPool;

    private static int collisionsCount = 0;
    private bool isActive = false;

    private void Start()
    {
        platformHitEffectPool = PoolManager.GetPoolByName("ComboColumnHitParticle");
    }

    public void Initialize(Platform platformRef)
    {
        platform = platformRef;
        transform.SetParent(platformRef.transform);
        platformHighlight.SetActive(true);
        collisionsCount = 0;
        isActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.gameObject.CompareTag("Player") && collisionsCount == 0 && Mathf.Abs(PlayerController.Position.x - transform.position.x) <= 0.7f)
        {
            PlayerController.OnComboPlatformHitted();
            platform.DisableComboLook();
            collisionsCount++;

            Disable();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collisionsCount = Mathf.Clamp(collisionsCount - 1, 0, 1);
        }
    }

    public void Disable()
    {
        platformHitEffectPool.GetPooledObject(transform.position);
        isActive = false;
        platformHighlight.SetActive(false);

        StartCoroutine(DisableCoroutine());
    }

    public static bool IsHittedComboPlatform()
    {
        bool result = collisionsCount == 1;

        if (!result)
        {
            collisionsCount = 0;
        }

        return result;
    }

    private IEnumerator DisableCoroutine()
    {
        gameObject.transform.SetParent(null);

        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);
    }

    public void FastDisable()
    {
        gameObject.transform.SetParent(null);
        gameObject.SetActive(false);
    }
}