using UnityEngine;
using Watermelon;

public class CoinBehaviour : MonoBehaviour
{
    public const float magnet_speed = 8f;
    public const float axceleration = 15f;

    public GameObject particleRef;
    private Pool pickUpParticlePool;
    private Pool pickUpParticleRelaxPool;
    private Transform transformRef;
    private float currentSpeed = 0f;
    private bool isInited = false;

    private void Awake()
    {
        transformRef = transform;
        currentSpeed = 0f;

        pickUpParticlePool = PoolManager.GetPoolByName("CoinPickUpParticle");
        pickUpParticleRelaxPool = PoolManager.GetPoolByName("CoinPickUpRelaxParticle");
    }


    public void MoveToPosition(Vector3 position)
    {
        if (currentSpeed < magnet_speed)
        {
            currentSpeed += axceleration * Time.deltaTime;
        }
        else if (currentSpeed < magnet_speed * 2f)
        {
            currentSpeed += axceleration * 0.5f * Time.deltaTime;
        }

        transformRef.position = Vector3.MoveTowards(transformRef.position, position, currentSpeed * Time.deltaTime);
    }

    public void OnEnable()
    {
        // if coin is on relax zone decreasing particles amount
        if(transformRef.position.y > 5)
        {
            // 30% chance to activate particle
            particleRef.SetActive(Random.Range(0f, 1f) < 0.3f);
        }
        else
        {
            particleRef.SetActive(true);
        }
    }

    public void OnDisable()
    {
        currentSpeed = 0;

        if (!isInited)
        {
            isInited = true;
            return;
        }


        if (transformRef.position.y < 5)
        {
            pickUpParticlePool.GetPooledObject(transformRef.position);
        }
        else
        {
            pickUpParticleRelaxPool.GetPooledObject(transformRef.position);
        }
    }
}