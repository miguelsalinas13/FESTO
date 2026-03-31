using UnityEngine;
using System.Collections;

public class BlinkGreenLights : MonoBehaviour
{
    [Header("Objects to blink")]
    public Renderer[] targetRenderers;

    [Header("Emission")]
    public Color emissionColor = Color.green;

    [Range(0f, 10f)]
    public float emissionIntensity = 3f;

    [Header("Blink Settings")]
    public float blinkInterval = 1f;

    private Coroutine blinkRoutine;
    private bool lightsOn = false;

    void OnDisable()
    {
        StopBlinking();
    }

    public void StartBlinking()
    {
        if (blinkRoutine == null)
        {
            blinkRoutine = StartCoroutine(BlinkLights());
        }
    }

    public void StopBlinking()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        lightsOn = false;
        TurnLightsOff();
    }

    IEnumerator BlinkLights()
    {
        while (true)
        {
            if (lightsOn)
                TurnLightsOff();
            else
                TurnLightsOn();

            lightsOn = !lightsOn;

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void TurnLightsOn()
    {
        Color finalColor = emissionColor * emissionIntensity;

        foreach (Renderer rend in targetRenderers)
        {
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", finalColor);
            }
        }
    }

    void TurnLightsOff()
    {
        foreach (Renderer rend in targetRenderers)
        {
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                mat.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}