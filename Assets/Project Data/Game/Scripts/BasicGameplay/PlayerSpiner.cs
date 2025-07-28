using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpiner : MonoBehaviour
{
    float sign = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ReferenceController.instance.playerController.rigidbodyRef.AddTorque(100f * sign, ForceMode2D.Force);
            sign *= -1f;
        }
    }
}