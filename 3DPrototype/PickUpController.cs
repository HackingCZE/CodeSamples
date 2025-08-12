using System;
using System.Collections;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField] private Transform pickUpSlot;
    [SerializeField] private float pickUpSpeed;
    [SerializeField] private float actionCooldown = 1f;
    [SerializeField] private float maxTrowForceMultiplayer = 3f;
    public IPickable ItemInSlot { get; private set; }
    public bool HasItemInSlot => ItemInSlot != null;
    public bool CanDoAction => Time.time > timer;
    public bool CanDoActionAndHasItem => CanDoAction && HasItemInSlot;
    public bool CanPickUp => pickUpSlot != null;
    private bool pickedUp = false;
    private bool trowing = false;

    PlayerController playerController;
    private float timer = 0;

    Coroutine trowCoroutine;

    public float ClickTime
    {
        get => Time.time - clickTime;
        private set => clickTime = value;
    }

    private float ProgressCharge => Mathf.Clamp((ClickTime / maxTrowForceMultiplayer) * 100, 0f, 100f);
    float clickTime = 0;
    public event Action<float> OnTrowing;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.OnInteractPerformed += Drop;
        playerController.OnClickStarted += StartTrow;
        playerController.OnClickCanceled += Trow;
    }

    void StartTrow()
    {
        if(!CanDoActionAndHasItem) return;
        Debug.Log("Trowing");
        clickTime = Time.time;
        trowing = true;
    }

    private void Trow()
    {
        if(!CanDoActionAndHasItem || !trowing) return;
        Debug.Log("Trowed");
        
        PerformAction();
        OnTrowing?.Invoke(0);
        ItemInSlot.Trow(playerController.PlayerDirection, Mathf.Clamp(clickTime, 0, maxTrowForceMultiplayer) + 1);
        ItemInSlot = null;
    }

    private void Drop()
    {
        if(!CanDoActionAndHasItem) return;
        PerformAction();
        ItemInSlot.Drop();
        ItemInSlot = null;
    }

    void PerformAction()
    {
        trowing = false;
        timer = Time.time + actionCooldown;
    }

    public void PickUp(IPickable pickable)
    {
        if(!CanDoAction || HasItemInSlot) return;
        PerformAction();
        ItemInSlot = pickable;

        StartCoroutine(TranslateObjectToSlot(pickable));
    }

    private void Update()
    {
        if(trowing)
        {
            OnTrowing?.Invoke(ProgressCharge);
        }

        if(!pickedUp || !HasItemInSlot) return;

        ItemInSlot.Item.transform.position = pickUpSlot.position;
    }

    IEnumerator TranslateObjectToSlot(IPickable pickable)
    {
        while(Vector3.Distance(pickable.Item.transform.position, pickUpSlot.position) > 0.1f)
        {
            pickable.Item.transform.position = Vector3.MoveTowards(pickable.Item.transform.position, pickUpSlot.position, pickUpSpeed * Time.deltaTime);
            yield return null;
        }

        pickable.Item.transform.position = pickUpSlot.position;
        pickedUp = true;
    }
}

public interface IPickable
{
    public GameObject Item { get; }
    public void PickUp();
    public void Drop();
    public void Trow(Vector3 throwDirection, float multiplayer);
}