using UnityEngine;

public class TurnOnYellowLights : MonoBehaviour
{
    [Header("Objects to light up")]
    public Renderer[] targetRenderers;

    [Header("Emission")]
    public Color emissionColor = Color.yellow;
    [Range(0f, 10f)]
    public float emissionIntensity = 2f;

    public void TurnLightsOn()
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

    public void TurnLightsOff()
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