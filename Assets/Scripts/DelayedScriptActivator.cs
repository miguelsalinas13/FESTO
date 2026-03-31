using UnityEngine;
using System.Collections;

public class DelayedScriptActivator : MonoBehaviour
{
    public MonoBehaviour scriptToActivate;
    public float delay = 3f;

    public void OnButtonPressed()
    {
        StartCoroutine(ActivateAfterDelay());
    }

    private IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (scriptToActivate != null)
        {
            scriptToActivate.enabled = true;
        }
    }
}
