using UnityEngine;

public class StepManager : MonoBehaviour
{
    [Header("Steps")]
    public int currentStep = 0;
    public string[] stepNames;

    private void Start()
    {
        ShowCurrentStep();
    }

    public void CompleteCurrentStep()
    {
        Debug.Log("Step Completed: " + GetCurrentStepName());

        currentStep++;

        if (currentStep >= stepNames.Length)
        {
            Debug.Log("Training Completed.");
        }
        else
        {
            ShowCurrentStep();
        }
    }

    public void ShowCurrentStep()
    {
        Debug.Log("Current Step [" + currentStep + "]: " + GetCurrentStepName());
    }

    public string GetCurrentStepName()
    {
        if (stepNames == null || stepNames.Length == 0)
            return "No Name";

        if (currentStep < 0 || currentStep >= stepNames.Length)
            return "Out of range";

        return stepNames[currentStep];
    }
}