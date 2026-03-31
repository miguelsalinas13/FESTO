using UnityEngine;

public class UIScreenStep : MonoBehaviour
{
    [Header("Screens in order")]
    public GameObject[] screens; // List of screens in order

    private int currentIndex = -1;

    void Start()
    {
        // Find which screen is currently active at the beginning
        for (int i = 0; i < screens.Length; i++)
        {
            if (screens[i] != null && screens[i].activeSelf)
            {
                currentIndex = i;
                break;
            }
        }
    }

    // This function will be called by the button
    public void GoToNextScreen()
    {
        // If no active screen was found, try to activate the first available one
        if (currentIndex == -1)
        {
            if (screens.Length > 0 && screens[0] != null)
            {
                screens[0].SetActive(true);
                currentIndex = 0;
            }

            return;
        }

        // Disable the current screen
        if (currentIndex >= 0 && currentIndex < screens.Length && screens[currentIndex] != null)
        {
            screens[currentIndex].SetActive(false);
        }

        // Move to the next screen
        currentIndex++;

        // Enable the next screen if it exists
        if (currentIndex < screens.Length && screens[currentIndex] != null)
        {
            screens[currentIndex].SetActive(true);
        }
        else
        {
            Debug.Log("End of screen sequence.");
        }
    }
}