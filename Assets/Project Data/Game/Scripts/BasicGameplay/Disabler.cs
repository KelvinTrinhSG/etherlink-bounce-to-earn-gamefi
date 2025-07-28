using UnityEngine;
using Watermelon;

public class Disabler : MonoBehaviour
{
    public Rigidbody2D rigidbodyRef;
    public Transform cameraTransform;

    public Vector3 cameraOffset;

    private bool freezeX;
    private bool freezeY;

    private float freezeValueX;
    private float freezeValueY;

    public bool FreezeX
    {
        get { return freezeX; }
        set
        {
            if (value)
                freezeValueX = transform.position.x;

            freezeX = value;
        }
    }

    public bool FreezeY
    {
        get { return freezeY; }
        set
        {
            if (value)
                freezeValueY = transform.position.y;

            freezeY = value;
        }
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = cameraTransform.position + cameraOffset;

        if (freezeY)
            newPosition = newPosition.SetY(freezeValueY);

        if (freezeX)
            newPosition = newPosition.SetX(freezeValueX);

        rigidbodyRef.MovePosition(newPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
    }
}