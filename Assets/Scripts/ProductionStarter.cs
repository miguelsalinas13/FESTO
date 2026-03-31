using System.Collections;
using UnityEngine;

public class ProductionStarter : MonoBehaviour
{
    public ProductionLineMover robotmo;
    public ProductionLineMover producto;
    public GameObject capsuleParent;

    public void StartAll()
    {
        StartCoroutine(StartAllRoutine());
    }

    private IEnumerator StartAllRoutine()
    {
        if (capsuleParent != null)
            capsuleParent.SetActive(true);

        yield return null;

        if (robotmo != null)
            robotmo.StartProduction();

        if (producto != null)
            producto.StartProduction();
    }
}