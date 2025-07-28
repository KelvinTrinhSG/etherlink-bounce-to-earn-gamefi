using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class CoinsMagnetBehaviour : MonoBehaviour
{
    public float effectDuration = 3f;

    private Transform playerTransform;
    private Rigidbody2D rigidbodyRef;

    private bool isActive;
    private float timeLeft;

    private List<CoinBehaviour> coinsList = new List<CoinBehaviour>();

    private void Awake()
    {
        playerTransform = ReferenceController.instance.playerController.transform;
        rigidbodyRef = GetComponent<Rigidbody2D>();
    }

    public void Activate()
    {
        if (isActive)
        {
            timeLeft = PowerUp.coinsMagnetDuration;
        }
        else
        {
            gameObject.SetActive(true);
            isActive = true;
            timeLeft = PowerUp.coinsMagnetDuration;

            StartCoroutine(MagnetBehaviourCoroutine());
            StartCoroutine(MagnetDurationController());
        }
    }


    public void Deactivate()
    {
        timeLeft = -1f;
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Coin"))
        {
            coinsList.Add(collision.GetComponent<CoinBehaviour>());
        }
    }

    private IEnumerator MagnetBehaviourCoroutine()
    {
        while (isActive || (!isActive && !coinsList.IsNullOrEmpty()))
        {
            // updating it's own position
            rigidbodyRef.MovePosition(playerTransform.position);


            // updating coins position
            int index = 0;
            while (index < coinsList.Count)
            {
                coinsList[index].MoveToPosition(playerTransform.position);

                if (!coinsList[index].gameObject.activeSelf)
                {
                    coinsList.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }


            // Debug.Log("UPdate");
            yield return null;
        }

        if (!isActive && coinsList.IsNullOrEmpty())
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MagnetDurationController()
    {
        while (timeLeft > 0)
        {
            timeLeft -= 1f;

            yield return new WaitForSeconds(1f);
        }

        Deactivate();
    }

}
