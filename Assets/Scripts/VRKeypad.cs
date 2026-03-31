using TMPro;
using UnityEngine;

public class VRKeypad : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text passwordDisplay;
    public TMP_Text messageText;

    [Header("UI Panels")]
    public GameObject keypadPanel;      // panel que contiene todo el keypad
    public GameObject successImage;     // imagen o panel que aparecerá al acertar

    [Header("Password")]
    public string correctPassword = "1234";
    public int maxLength = 8;
    public bool hideCharacters = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip keySound;
    public AudioClip successSound;
    public AudioClip errorSound;
    public AudioClip clearSound;
    public AudioClip backspaceSound;

    [Header("Step Manager")]
    public StepManager stepManager;
    public int loginStepIndex = 1;

    private string currentInput = "";

    public void PressKey(string key)
    {
        if (currentInput.Length >= maxLength)
            return;

        currentInput += key;
        RefreshDisplay();
        PlaySound(keySound);
    }

    public void ClearInput()
    {
        currentInput = "";
        RefreshDisplay();

        if (messageText != null)
            messageText.text = "";

        PlaySound(clearSound != null ? clearSound : keySound);
    }

    public void Backspace()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            RefreshDisplay();
            PlaySound(backspaceSound != null ? backspaceSound : keySound);
        }
    }

    public void ConfirmInput()
    {
        if (currentInput == correctPassword)
        {
            if (messageText != null)
                messageText.text = "Access granted";

            Debug.Log("Correct password");

            PlaySound(successSound != null ? successSound : keySound);

            // ocultar keypad
            if (keypadPanel != null)
                keypadPanel.SetActive(false);

            // mostrar imagen o nuevo panel
            if (successImage != null)
                successImage.SetActive(true);

            // avanzar al siguiente step solo si estamos en el step correcto
            if (stepManager != null && stepManager.currentStep == loginStepIndex)
            {
                stepManager.CompleteCurrentStep();
            }
        }
        else
        {
            if (messageText != null)
                messageText.text = "Wrong password";

            Debug.Log("Wrong password");

            PlaySound(errorSound != null ? errorSound : keySound);
        }
    }

    private void RefreshDisplay()
    {
        if (passwordDisplay == null)
            return;

        if (hideCharacters)
            passwordDisplay.text = new string('*', currentInput.Length);
        else
            passwordDisplay.text = currentInput;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}