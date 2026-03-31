using UnityEngine;
using UnityEngine.InputSystem;
using TS.GazeInteraction;

public class KeyTurnHold : MonoBehaviour
{
    [Header("Rotation")]
    public Transform key;
    public float rotationSpeed = 120f;
    public float maxAngle = 90f;
    public Vector3 localRotationAxis = Vector3.up;

    [Header("Interaction")]
    public GazeInteractable gazeInteractable;
    public InputActionReference triggerAction;

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

    private bool isLooking = false;
    private bool turning = false;

    private float currentAngle = 0f;
    private float targetAngle = 0f;

    private Quaternion startLocalRotation;
    private bool wasTriggerPressed = false;

    void Start()
    {
        if (key != null)
            startLocalRotation = key.localRotation;

        if (greenLight != null)
        {
            greenLight.material.EnableKeyword("_EMISSION");
            greenLight.material.SetColor("_EmissionColor", Color.black);
        }
    }

    void OnEnable()
    {
        if (gazeInteractable != null)
        {
            gazeInteractable.Enter += HandleEnter;
            gazeInteractable.Exit += HandleExit;
        }

        if (triggerAction != null)
            triggerAction.action.Enable();
    }

    void OnDisable()
    {
        if (gazeInteractable != null)
        {
            gazeInteractable.Enter -= HandleEnter;
            gazeInteractable.Exit -= HandleExit;
        }

        if (triggerAction != null)
            triggerAction.action.Disable();
    }

    void Update()
    {
        if (key == null) return;

        bool triggerPressed = false;

        if (triggerAction != null)
            triggerPressed = triggerAction.action.ReadValue<float>() > 0.1f;

        bool triggerJustPressed = triggerPressed && !wasTriggerPressed;

        if (isLooking && triggerJustPressed && !turning)
        {
            turning = true;

            if (Mathf.Approximately(currentAngle, 0f))
                targetAngle = maxAngle;
            else
                targetAngle = 0f;

            if (audioSource != null && keySound != null)
                audioSource.PlayOneShot(keySound);
        }

        if (turning)
        {
            currentAngle = Mathf.MoveTowards(
                currentAngle,
                targetAngle,
                rotationSpeed * Time.deltaTime
            );

            key.localRotation =
                startLocalRotation *
                Quaternion.AngleAxis(currentAngle, localRotationAxis.normalized);

            if (Mathf.Approximately(currentAngle, targetAngle))
            {
                turning = false;

                if (greenLight != null)
                {
                    Color c = Mathf.Approximately(targetAngle, maxAngle)
                        ? Color.green * 3f
                        : Color.black;

                    greenLight.material.EnableKeyword("_EMISSION");
                    greenLight.material.SetColor("_EmissionColor", c);
                }

                if (Mathf.Approximately(targetAngle, maxAngle))
                {
                    if (stepManager != null && stepManager.currentStep == stepIndex)
                    {
                        stepManager.CompleteCurrentStep();
                    }
                }

                if (Mathf.Approximately(targetAngle, 0f))
                {
                    TurnOffSpecificYellowLight();
                }
            }
        }

        wasTriggerPressed = triggerPressed;
    }

    void HandleEnter(GazeInteractable i, GazeInteractor g, Vector3 p)
    {
        isLooking = true;
    }

    void HandleExit(GazeInteractable i, GazeInteractor g)
    {
        isLooking = false;
    }

    void TurnOffSpecificYellowLight()
    {
        if (yellowBlockToTurnOff == null) return;

        foreach (Material mat in yellowBlockToTurnOff.materials)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
}