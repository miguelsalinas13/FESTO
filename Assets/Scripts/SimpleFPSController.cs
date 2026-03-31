using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Rotation")]
    public float yawSpeed = 90f;     // izquierda/derecha
    public float pitchSpeed = 60f;   // arriba/abajo
    public float minPitch = -60f;
    public float maxPitch = 60f;

    [Header("References")]
    public Transform cameraTransform;

    [Header("Key Bindings")]

    public Key forwardKey = Key.W;
    public Key backwardKey = Key.S;
    public Key leftKey = Key.A;
    public Key rightKey = Key.D;

    public Key turnLeftKey = Key.Q;
    public Key turnRightKey = Key.E;

    public Key lookUpKey = Key.UpArrow;
    public Key lookDownKey = Key.DownArrow;

    private float pitch = 0f;

    void Start()
    {
        // Mouse libre
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // =====================
        // MOVEMENT
        // =====================
        Vector3 move = Vector3.zero;

        if (Keyboard.current[forwardKey].isPressed) move += transform.forward;
        if (Keyboard.current[backwardKey].isPressed) move -= transform.forward;
        if (Keyboard.current[leftKey].isPressed) move -= transform.right;
        if (Keyboard.current[rightKey].isPressed) move += transform.right;

        transform.position += move.normalized * moveSpeed * Time.deltaTime;

        // =====================
        // YAW (izquierda/derecha)
        // =====================
        float yaw = 0f;

        if (Keyboard.current[turnLeftKey].isPressed) yaw -= 1f;
        if (Keyboard.current[turnRightKey].isPressed) yaw += 1f;

        transform.Rotate(Vector3.up * yaw * yawSpeed * Time.deltaTime);

        // =====================
        // PITCH (arriba/abajo)
        // =====================
        float pitchInput = 0f;

        if (Keyboard.current[lookUpKey].isPressed) pitchInput += 1f;
        if (Keyboard.current[lookDownKey].isPressed) pitchInput -= 1f;

        pitch += pitchInput * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}