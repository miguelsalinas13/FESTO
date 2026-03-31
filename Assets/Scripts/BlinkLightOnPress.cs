using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BlinkLightOnPress : MonoBehaviour
{
    [Header("Raycast")]
    public Transform rayOrigin;
    public float rayDistance = 5f;
    public LayerMask buttonLayer;

    [Header("Input")]
    public InputActionReference triggerAction;

    [Header("Blink Object")]
    public Renderer targetRenderer;   // objeto que titila
    public float blinkInterval = 1f;  // tiempo entre encendido/apagado

    [Header("Materials")]
    public Material onMaterial;
    public Material offMaterial;

    private bool isHovering = false;
    private bool blinking = false;
    private Coroutine blinkRoutine;

    void OnEnable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.Enable();
            triggerAction.action.performed += OnTriggerPressed;
        }
    }

    void OnDisable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed -= OnTriggerPressed;
            triggerAction.action.Disable();
        }
    }

    void Update()
    {
        CheckRaycast();
    }

    void CheckRaycast()
    {
        isHovering = false;

        if (rayOrigin == null)
            return;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, buttonLayer))
        {
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                isHovering = true;
            }
        }
    }

    void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (!isHovering)
            return;

        if (!blinking)
        {
            blinkRoutine = StartCoroutine(BlinkLoop());
            blinking = true;
        }
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            targetRenderer.material = onMaterial;
            yield return new WaitForSeconds(blinkInterval);

            targetRenderer.material = offMaterial;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}