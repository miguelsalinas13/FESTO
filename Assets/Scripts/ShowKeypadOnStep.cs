using UnityEngine;

public class ShowKeypadOnStep : MonoBehaviour
{
    public GameObject keypad;

    public void ShowKeypad()
    {
        if (keypad != null)
            keypad.SetActive(true);
    }

    public void HideKeypad()
    {
        if (keypad != null)
            keypad.SetActive(false);
    }
}