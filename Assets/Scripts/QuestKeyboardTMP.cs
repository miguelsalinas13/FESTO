using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestKeyboardTMP : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_InputField inputField;

    private TouchScreenKeyboard keyboard;

    public void OnSelect(BaseEventData eventData)
    {
        if (!TouchScreenKeyboard.isSupported) return;

        keyboard = TouchScreenKeyboard.Open(
            inputField.text,
            TouchScreenKeyboardType.Default,
            false,   // autocorrection
            false,   // multiline
            false,   // secure/password
            false    // alert
        );
    }

    private void Update()
    {
        if (keyboard == null) return;

        inputField.text = keyboard.text;

        if (keyboard.status == TouchScreenKeyboard.Status.Done ||
            keyboard.status == TouchScreenKeyboard.Status.Canceled)
        {
            keyboard = null;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        keyboard = null;
    }
}