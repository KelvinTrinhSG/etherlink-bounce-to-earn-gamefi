using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectController : MonoBehaviour
{
    public ParticleSystem particle;

    public void Init( ParticleSystem particleRef)
    {
        if(particle != null)
        {
            particle.transform.SetParent(null);
            particle.gameObject.SetActive(false);
        }

        particle = particleRef;
        particle.transform.SetParent(transform);
        particle.transform.localPosition = new Vector3();
        particle.gameObject.SetActive(true);
    }

    public void Play(Vector3 position)
    {
        transform.position = position;

        particle.Play();
    }
}