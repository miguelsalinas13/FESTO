using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VRKeypad : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text passwordDisplay;
    public TMP_Text messageText;

    [Header("UI Panels")]
    public GameObject keypadPanel;
    public GameObject successImage;

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

    [Header("Visual Feedback")]
    public Color pressedColor = Color.green;
    public float pressedDuration = 0.15f;

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

    public void PressKeyWithVisual(string key, Image buttonImage)
    {
        PressKey(key);

        if (buttonImage != null)
            StartCoroutine(FlashUIButton(buttonImage));
    }

    public void ClearInput()
    {
        currentInput = "";
        RefreshDisplay();

        if (messageText != null)
            messageText.text = "";

        PlaySound(clearSound != null ? clearSound : keySound);
    }

    public void ClearInputWithVisual(Image buttonImage)
    {
        ClearInput();

        if (buttonImage != null)
            StartCoroutine(FlashUIButton(buttonImage));
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

    public void BackspaceWithVisual(Image buttonImage)
    {
        Backspace();

        if (buttonImage != null)
            StartCoroutine(FlashUIButton(buttonImage));
    }

    public void ConfirmInput()
    {
        if (currentInput == correctPassword)
        {
            if (messageText != null)
                messageText.text = "Access granted";

            Debug.Log("Correct password");
            PlaySound(successSound != null ? successSound : keySound);

            if (keypadPanel != null)
                keypadPanel.SetActive(false);

            if (successImage != null)
                successImage.SetActive(true);

            if (stepManager != null && stepManager.currentStep == loginStepIndex)
                stepManager.CompleteCurrentStep();
        }
        else
        {
            if (messageText != null)
                messageText.text = "Wrong password";

            Debug.Log("Wrong password");
            PlaySound(errorSound != null ? errorSound : keySound);
        }
    }

    public void ConfirmInputWithVisual(Image buttonImage)
    {
        ConfirmInput();

        if (buttonImage != null)
            StartCoroutine(FlashUIButton(buttonImage));
    }

    private void RefreshDisplay()
    {
        if (passwordDisplay == null)
            return;

        passwordDisplay.text = hideCharacters
            ? new string('*', currentInput.Length)
            : currentInput;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private IEnumerator FlashUIButton(Image buttonImage)
    {
        Color originalColor = buttonImage.color;
        buttonImage.color = pressedColor;

        yield return new WaitForSeconds(pressedDuration);

        buttonImage.color = originalColor;
    }
}