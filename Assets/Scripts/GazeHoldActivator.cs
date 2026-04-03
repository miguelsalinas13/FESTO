using UnityEngine;
using TS.GazeInteraction;

public class GazeHoldActivator : MonoBehaviour
{
    [Header("Gaze")]
    public GazeInteractable gazeInteractable;

    [Header("Timing")]
    public float holdTime = 2f;

    [Header("Objects To Activate")]
    public GameObject[] objectsToActivate;

    [Header("Options")]
    public bool onlyOnce = true;
    public bool deactivateOnExit = false;

    private float timer = 0f;
    private bool isLooking = false;
    private bool alreadyActivated = false;

    private void Awake()
    {
        if (gazeInteractable == null)
            gazeInteractable = GetComponent<GazeInteractable>();
    }

    private void OnEnable()
    {
        if (gazeInteractable != null)
        {
            gazeInteractable.Enter += OnGazeEnter;
            gazeInteractable.Exit += OnGazeExit;
        }
    }

    private void OnDisable()
    {
        if (gazeInteractable != null)
        {
            gazeInteractable.Enter -= OnGazeEnter;
            gazeInteractable.Exit -= OnGazeExit;
        }
    }

    private void Update()
    {
        if (!isLooking)
            return;

        if (onlyOnce && alreadyActivated)
            return;

        timer += Time.deltaTime;

        if (timer >= holdTime)
        {
            ActivateObjects();
        }
    }

    private void OnGazeEnter(GazeInteractable interactable, GazeInteractor interactor, Vector3 point)
    {
        if (onlyOnce && alreadyActivated)
            return;

        isLooking = true;
        timer = 0f;
    }

    private void OnGazeExit(GazeInteractable interactable, GazeInteractor interactor)
    {
        isLooking = false;
        timer = 0f;

        if (deactivateOnExit && (!onlyOnce || !alreadyActivated))
        {
            DeactivateObjects();
        }
    }

    private void ActivateObjects()
    {
        isLooking = false;
        timer = 0f;
        alreadyActivated = true;

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        Debug.Log("GazeHoldActivator: Activated");
    }

    private void DeactivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}