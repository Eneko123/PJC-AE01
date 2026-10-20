using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    public bool IsHeld { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        IsHeld = true;
        rb.isKinematic = true;
        col.enabled = false;               // evita que empuje al jugador mientras la lleva
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    public void Throw(Vector3 velocity)
    {
        IsHeld = false;
        transform.SetParent(null);
        col.enabled = true;
        rb.isKinematic = false;
        rb.linearVelocity = velocity;
    }

    public void Score() => gameObject.SetActive(false);
}