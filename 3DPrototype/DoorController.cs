using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    private Animator animator;
    private bool isOpen = false;
    [SerializeField] AreaTrigger areaToCloseDoor;
    [SerializeField] Collider blockCollider;

    private LayerMask playerLayer => LayerMask.NameToLayer("Player");
    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        areaToCloseDoor.OnTriggerEnterEvent += OnTriggerEnter;
    }

    private void OnTriggerEnter(Collider obj)
    {
        if(obj.gameObject.layer == playerLayer)
        {
            StartCoroutine(CloseDoor());
        }
    }

    IEnumerator CloseDoor()
    {
        blockCollider.enabled = true;
        yield return new WaitForFixedUpdate();
        animator.SetTrigger("Close");
        Destroy(areaToCloseDoor.gameObject);
        Destroy(this);
    }

    public string TextToShow => !isOpen ? "Open" : "";

    public void Interact()
    {
        if(!isOpen)
        {
            isOpen = true;
            blockCollider.enabled = false;
            animator.SetTrigger("Open");
        }
    }
}

public interface IInteractable
{
    public string TextToShow { get;}
    public void Interact();
}
