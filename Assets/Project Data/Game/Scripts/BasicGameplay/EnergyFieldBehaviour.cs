using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyFieldBehaviour : MonoBehaviour
{
    public GameObject normalParticleObject;
    public GameObject onBecameActiveParticle;


    public void OnPickedUp()
    {
        onBecameActiveParticle.SetActive(true);
    }
}
