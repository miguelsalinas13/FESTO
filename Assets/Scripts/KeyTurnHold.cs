using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class KeyTurnButton : MonoBehaviour
{
    [Header("XR Interaction")]
    public XRSimpleInteractable interactable;

    [Header("Input")]
    public InputActionReference triggerAction;

    [Header("Optional Filter")]
    public XRRayInteractor allowedRayInteractor;

    [Header("Rotation")]
    public Transform key;
    public float rotationSpeed = 120f;
    public float maxAngle = 90f;
    public Vector3 localRotationAxis = Vector3.up;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip keySound;

    [Header("Feedback")]
    public Renderer greenLight;

    [Header("Specific Yellow Light To Turn Off")]
    public Renderer yellowBlockToTurnOff;

    [Header("Step Manager")]
    public StepManager stepManager;
    public int stepIndex = 4;

    private bool turning = false;
    private float currentAngle = 0f;
    private float targetAngle = 0f;
    private Quaternion startLocalRotation;

    private bool isHoveredByAllowedInteractor = false;

    void Start()
    {
        if (key != null)
            startLocalRotation = key.localRotation;

        if (greenLight != null)
        {
            foreach (Material mat in greenLight.materials)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    void OnEnable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
        }

        if (triggerAction != null && triggerAction.action != null)
            triggerAction.action.Enable();
    }

    void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
        }

        if (triggerAction != null && triggerAction.action != null)
            triggerAction.action.Disable();
    }

    void Update()
    {
        if (triggerAction != null &&
            triggerAction.action != null &&
            isHoveredByAllowedInteractor &&
            !turning &&
            triggerAction.action.WasPressedThisFrame())
        {
            TurnKey();
        }

        if (key == null || !turning)
            return;

        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        key.localRotation = startLocalRotation *
                            Quaternion.AngleAxis(currentAngle, localRotationAxis.normalized);

        if (Mathf.Abs(currentAngle - targetAngle) < 0.01f)
        {
            currentAngle = targetAngle;
            turning = false;

            UpdateGreenLight();

            if (Mathf.Approximately(targetAngle, maxAngle))
            {
                if (stepManager != null && stepManager.currentStep == stepIndex)
                    stepManager.CompleteCurrentStep();
            }

            if (Mathf.Approximately(targetAngle, 0f))
            {
                TurnOffSpecificYellowLight();
            }
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (allowedRayInteractor == null)
        {
            isHoveredByAllowedInteractor = true;
            return;
        }

        if (args.interactorObject is XRRayInteractor rayInteractor &&
            rayInteractor == allowedRayInteractor)
        {
            isHoveredByAllowedInteractor = true;
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (allowedRayInteractor == null)
        {
            isHoveredByAllowedInteractor = false;
            return;
        }

        if (args.interactorObject is XRRayInteractor rayInteractor &&
            rayInteractor == allowedRayInteractor)
        {
            isHoveredByAllowedInteractor = false;
        }
    }

    private void TurnKey()
    {
        if (Mathf.Approximately(currentAngle, 0f))
            targetAngle = maxAngle;
        else
            targetAngle = 0f;

        turning = true;

        if (audioSource != null && keySound != null)
            audioSource.PlayOneShot(keySound);
    }

    private void UpdateGreenLight()
    {
        if (greenLight == null)
            return;

        Color emissionColor = Mathf.Approximately(targetAngle, maxAngle)
            ? Color.green * 3f
            : Color.black;

        foreach (Material mat in greenLight.materials)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissionColor);
        }
    }

    private void TurnOffSpecificYellowLight()
    {
        if (yellowBlockToTurnOff == null)
            return;

        foreach (Material mat in yellowBlockToTurnOff.materials)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
}