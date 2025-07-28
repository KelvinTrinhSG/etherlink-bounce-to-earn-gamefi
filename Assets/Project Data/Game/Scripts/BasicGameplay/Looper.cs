using UnityEngine;
using Watermelon;

public class Looper : MonoBehaviour
{
    public Rigidbody2D rigidbodyRef;
    public Transform cameraTransform;

    public Vector3 cameraOffset;

    private void FixedUpdate()
    {
        rigidbodyRef.MovePosition(cameraTransform.position + cameraOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 newPosition =  collision.gameObject.transform.position.AddToX(50f);
        collision.gameObject.transform.position = newPosition;
    }
}