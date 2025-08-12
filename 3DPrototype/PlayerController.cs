using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public InputSystem_Actions inputActions;
    private Rigidbody rigidBody;
    private CapsuleCollider collider;
    [Header("Movement")] public float speed = 5f;

    [Header("Jump")] [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [Header("Look")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float lookSensitivity = 1;
    float yRotation = 0, xRotation = 0;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = .2f;

    [SerializeField] private LayerMask interactionLayer;
    bool isMoving = false;

    Vector3 moveInput = Vector3.zero;
    Vector2 lookInput = Vector2.zero;

    public PickUpController pickUpController;
    
    public Vector3 PlayerDirection => cameraHolder.forward;
    
    // Events
    public event Action<IInteractable> OnInteractHover;
    public event Action OnInteractPerformed;
    public event Action OnClickStarted;
    public event Action OnClickCanceled;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        rigidBody = GetComponent<Rigidbody>();
        inputActions = new InputSystem_Actions();
        pickUpController = GetComponent<PickUpController>();
        collider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Look.performed += LookOnPerformed;
        inputActions.Player.Look.canceled += LookOnCanceled;

        inputActions.Player.Interact.started += InteractOnPerformed;
        inputActions.UI.Click.started += ClickOnStarted;
        inputActions.UI.Click.canceled += ClickOnCanceled;

        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void ClickOnStarted(InputAction.CallbackContext obj)
    {
        OnClickStarted?.Invoke();
    }
    private void ClickOnCanceled(InputAction.CallbackContext obj)
    {
        OnClickCanceled?.Invoke();
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        var interactable = CheckInteractInterface<IInteractable>();
        interactable?.Interact();

        var pickable = CheckInteractInterface<IPickable>();
        pickable?.PickUp();
        
        OnInteractPerformed?.Invoke();
    }

    private T CheckInteractInterface<T>() where T : class
    {
        if(pickUpController.ItemInSlot != null) return null;
        
        if(Physics.Raycast(transform.position, PlayerDirection, out RaycastHit hit, interactDistance, interactionLayer))
        {
            if(hit.collider.gameObject.TryGetComponent<T>(out T interactable))
            {
                return interactable;
            }
        }

        return null;
    }


    private void LookOnPerformed(InputAction.CallbackContext obj)
    {
        lookInput = obj.ReadValue<Vector2>();
    }

    private void LookOnCanceled(InputAction.CallbackContext obj)
    {
        lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Look.performed -= LookOnPerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Interact.started -= InteractOnPerformed;
        inputActions.UI.Click.started -= ClickOnStarted;
        inputActions.UI.Click.canceled -= ClickOnCanceled;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if(isGrounded)
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        CheckGround();

        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        movement = cameraHolder.rotation * movement;
        movement.y = 0;

        rigidBody.linearVelocity = new Vector3(movement.x * speed, rigidBody.linearVelocity.y, movement.z * speed);

        var hit = CheckInteractInterface<IInteractable>();

        OnInteractHover?.Invoke(hit);
    }

    private void Update()
    {
        UpdateCamera();
        UpdatePlayerScale();
    }

    private void UpdatePlayerScale()
    {
        bool isPlayerLookBehind = cameraHolder.rotation.eulerAngles.y is > 145 and < 260;
        collider.radius = !isPlayerLookBehind ? .5f : .2f;
    }

    private void UpdateCamera()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, interactDistance * PlayerDirection);
    }
}