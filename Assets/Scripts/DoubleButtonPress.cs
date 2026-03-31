using UnityEngine;
using UnityEngine.UI;

public class DoubleButtonPress : MonoBehaviour
{
    public Button targetButton;

    public void PressThreeTimes()
    {
        if (targetButton == null)
            return;

        targetButton.onClick.Invoke();
        targetButton.onClick.Invoke();
        targetButton.onClick.Invoke();
        targetButton.onClick.Invoke();
    }
}