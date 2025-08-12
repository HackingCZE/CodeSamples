using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rock : MonoBehaviour, IPickable, IInteractable
{
    public GameObject Item => gameObject;
    private Rigidbody rb;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] LayerMask collisionMask;

    bool trowing = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PickUp()
    {
        rb.useGravity = false;
        PlayerController.Instance.pickUpController.PickUp(this);
    }

    public void Drop()
    {
        rb.useGravity = true;
    }

    public void Trow(Vector3 throwDirection, float multiplayer)
    {
        Drop();
        trowing = true;
        rb.AddForce(throwDirection * (throwForce * multiplayer), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if((collisionMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if(other.gameObject.TryGetComponent<IRockHittable>(out IRockHittable hittable))
            {
                hittable.Hit();
            }
        }
    }

    public string TextToShow => PlayerController.Instance.pickUpController.ItemInSlot != (IPickable)this ? "Pick Up" : "";
    public void Interact()
    {
        return;
    }
}

public interface IRockHittable
{
    public void Hit();
}