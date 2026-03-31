using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VRStepButton : MonoBehaviour
{
    [Header("Raycast")]
    public Transform rayOrigin;
    public float rayDistance = 5f;
    public LayerMask buttonLayer;

    [Header("Input")]
    public InputActionReference triggerAction;

    [Header("Step Manager")]
    public StepManager stepManager;     // Reference to the global step manager
    public int requiredStep = 0;        // Step index required to activate this button

    [Header("Button Parts")]
    public Transform buttonTop;         // Top part of the button that moves down
    public Renderer highlightRenderer;  // Renderer used for hover highlight
    public Renderer successRenderer;    // Renderer used for success state

    [Header("Materials")]
    public Material normalMaterial;
    public Material highlightMaterial;
    public Material successMaterial;

    [Header("Animation")]
    public float pressDepth = 0.008f;
    public float pressSpeed = 12f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("State")]
    public bool oneShot = true;         // If true, the button can only be completed once
    public bool stepCompleted = false;

    [Header("Events")]
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    public UnityEvent onPressed;
    public UnityEvent onStepCompleted;

    private Vector3 initialLocalPos;
    private bool isHovering = false;
    private bool isPressedVisual = false;
    private bool wasHoveringLastFrame = false;

    private void Start()
    {
        // Store the initial local position of the moving top part
        if (buttonTop != null)
            initialLocalPos = buttonTop.localPosition;

        // Apply the default visual state
        SetNormalVisual();
    }

    private void OnEnable()
    {
        // Subscribe to trigger input events
        if (triggerAction != null && triggerAction.action != null)
        {
            triggerAction.action.Enable();
            triggerAction.action.performed += OnTriggerPressed;
            triggerAction.action.canceled += OnTriggerReleased;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from trigger input events
        if (triggerAction != null && triggerAction.action != null)
        {
            triggerAction.action.performed -= OnTriggerPressed;
            triggerAction.action.canceled -= OnTriggerReleased;
            triggerAction.action.Disable();
        }
    }

    private void Update()
    {
        CheckRaycast();
        HandleHoverEvents();
        AnimateButton();
    }

    private void CheckRaycast()
    {
        isHovering = false;

        // Stop checking if there is no ray origin or if the step is already completed
        if (rayOrigin == null || stepCompleted)
            return;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        // Check if the ray hits an object in the button layer
        if (Physics.Raycast(ray, out hit, rayDistance, buttonLayer))
        {
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                isHovering = true;
            }
        }
    }

    private void HandleHoverEvents()
    {
        // Do not process hover visuals if the step is already completed
        if (stepCompleted)
            return;

        if (isHovering && !wasHoveringLastFrame)
        {
            SetHighlightVisual();
            onHoverEnter.Invoke();
        }
        else if (!isHovering && wasHoveringLastFrame)
        {
            SetNormalVisual();
            onHoverExit.Invoke();
        }

        wasHoveringLastFrame = isHovering;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        // Prevent interaction if the button is already completed and is one-shot
        if (stepCompleted && oneShot)
            return;

        // Only allow press if the button is currently being pointed at
        if (!isHovering)
            return;

        // Validate the current required step
        if (stepManager != null && stepManager.currentStep != requiredStep)
        {
            Debug.Log("Wrong step. This button is not active yet.");
            return;
        }

        isPressedVisual = true;

        // Play click sound
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Invoke custom press event
        onPressed.Invoke();

        // Complete this button step
        CompleteStep();
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        isPressedVisual = false;
    }

    private void CompleteStep()
    {
        // Prevent duplicate completion if one-shot is enabled
        if (stepCompleted && oneShot)
            return;

        stepCompleted = true;
        isHovering = false;
        wasHoveringLastFrame = false;

        // Apply success material
        SetSuccessVisual();

        // Notify external listeners
        onStepCompleted.Invoke();

        // Notify the step manager directly
        if (stepManager != null)
        {
            stepManager.CompleteCurrentStep();
        }

        Debug.Log("Button step completed: " + gameObject.name);
    }

    private void AnimateButton()
    {
        if (buttonTop == null)
            return;

        Vector3 targetPos = initialLocalPos;

        // Move the top part down while pressed or after completion
        if (isPressedVisual || stepCompleted)
        {
            targetPos = initialLocalPos + new Vector3(0f, -pressDepth, 0f);
        }

        buttonTop.localPosition = Vector3.Lerp(
            buttonTop.localPosition,
            targetPos,
            Time.deltaTime * pressSpeed
        );
    }

    private void SetNormalVisual()
    {
        if (stepCompleted)
            return;

        if (highlightRenderer != null && normalMaterial != null)
            highlightRenderer.material = normalMaterial;
    }

    private void SetHighlightVisual()
    {
        if (stepCompleted)
            return;

        if (highlightRenderer != null && highlightMaterial != null)
            highlightRenderer.material = highlightMaterial;
    }

    private void SetSuccessVisual()
    {
        if (successRenderer != null && successMaterial != null)
            successRenderer.material = successMaterial;
    }

    public void ResetButton()
    {
        // Reset internal button state
        stepCompleted = false;
        isHovering = false;
        wasHoveringLastFrame = false;
        isPressedVisual = false;

        // Restore the initial top position
        if (buttonTop != null)
            buttonTop.localPosition = initialLocalPos;

        // Restore normal material
        SetNormalVisual();
    }

    public bool IsCompleted()
    {
        return stepCompleted;
    }
}