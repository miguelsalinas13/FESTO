using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeyTurnButton : MonoBehaviour
{
    [Header("XR Interaction")]
    public XRSimpleInteractable interactable;

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
        if (interactable != null)
            interactable.selectEntered.AddListener(OnKeyPressed);
    }

    void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnKeyPressed);
    }

    void Update()
    {
        if (key == null || !turning) return;

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
                    stepManager.CompleteCurrentStep();
            }

            if (Mathf.Approximately(targetAngle, 0f))
            {
                TurnOffSpecificYellowLight();
            }
        }
    }

    private void OnKeyPressed(SelectEnterEventArgs args)
    {
        if (turning) return;

        if (Mathf.Approximately(currentAngle, 0f))
            targetAngle = maxAngle;
        else
            targetAngle = 0f;

        turning = true;

        if (audioSource != null && keySound != null)
            audioSource.PlayOneShot(keySound);
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