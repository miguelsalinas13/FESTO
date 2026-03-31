using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TS.GazeInteraction;

public class BarometerCheck : MonoBehaviour
{
    [Header("Gaze")]
    public GazeInteractable gazeInteractable;

    [Header("Timing")]
    public float showHourglassAfter = 1f;
    public float validateAfter = 4f;

    [Header("UI")]
    public GameObject hourglassObject;   // imagen o icono de reloj de arena
    public TMP_Text messageText;         // texto tipo "Pressure correct"

    [Header("Step Manager")]
    public StepManager stepManager;
    public int barometerStepIndex = 2;   // cambia esto según tu step real

    private float gazeTimer = 0f;
    private bool isLooking = false;
    private bool alreadyValidated = false;

    private void Awake()
    {
        if (gazeInteractable == null)
            gazeInteractable = GetComponent<GazeInteractable>();
    }

    private void OnEnable()
    {
        if (gazeInteractable != null)
        {
            gazeInteractable.Enter += HandleGazeEnter;
            gazeInteractable.Exit += HandleGazeExit;
        }
    }

    private void OnDisable()
    {
        if (gazeInteractable != null)
        {
            gazeInteractable.Enter -= HandleGazeEnter;
            gazeInteractable.Exit -= HandleGazeExit;
        }
    }

    private void Start()
    {
        if (hourglassObject != null)
            hourglassObject.SetActive(false);

        if (messageText != null)
            messageText.text = "";
    }

    private void Update()
    {
        if (!isLooking || alreadyValidated)
            return;

        gazeTimer += Time.deltaTime;

        if (gazeTimer >= showHourglassAfter && hourglassObject != null)
            hourglassObject.SetActive(true);

        if (gazeTimer >= validateAfter)
        {
            ValidateBarometer();
        }
    }

    private void HandleGazeEnter(GazeInteractable interactable, GazeInteractor interactor, Vector3 point)
    {
        if (alreadyValidated)
            return;

        isLooking = true;
        gazeTimer = 0f;

        if (hourglassObject != null)
            hourglassObject.SetActive(false);

        if (messageText != null)
            messageText.text = "";
    }

    private void HandleGazeExit(GazeInteractable interactable, GazeInteractor interactor)
    {
        if (alreadyValidated)
            return;

        ResetCheck();
    }

    private void ValidateBarometer()
    {
        alreadyValidated = true;
        isLooking = false;

        if (hourglassObject != null)
            hourglassObject.SetActive(false);

        if (messageText != null)
            messageText.text = "Pressure correct";

        Debug.Log("Pressure correct");

        if (stepManager != null && stepManager.currentStep == barometerStepIndex)
        {
            stepManager.CompleteCurrentStep();
        }
    }

    private void ResetCheck()
    {
        isLooking = false;
        gazeTimer = 0f;

        if (hourglassObject != null)
            hourglassObject.SetActive(false);

        if (messageText != null)
            messageText.text = "";
    }
}