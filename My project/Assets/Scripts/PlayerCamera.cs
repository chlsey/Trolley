using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensX = 0.1f;
    public float mouseSensY = 0.1f;
    public float controllerSensX = 150f;
    public float controllerSensY = 150f;

    [Header("References")]
    public Transform orientation;

    [Header("Rotation")]
    public float xRotation;
    public float yRotation;

    [Header("Clamp Settings")]
    public bool clampView = false;
    public float minX = -90f;
    public float maxX = 90f;
    public float minY = -360f;
    public float maxY = 360f;

    public float tiltZ = 0f;

    private bool isUsingGamepad = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        mouseSensX = settings.mouseSens;
        mouseSensY = settings.mouseSens;
        controllerSensX = settings.controllerSens;
        controllerSensY = settings.controllerSens;
    }

    private void OnEnable()
    {
        SettingsManager.Instance.SettingsChanged += ApplySettings;
    }

    private void OnDisable()
    {
        SettingsManager.Instance.SettingsChanged -= ApplySettings;
    }

    private void Update()
    {
        if (PauseMenuController.IsPaused) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        Vector2 stickDelta = Gamepad.current != null ? Gamepad.current.rightStick.ReadValue() : Vector2.zero;

        if (mouseDelta.sqrMagnitude > 0.01f)
            isUsingGamepad = false;

        if (stickDelta.sqrMagnitude > 0.01f)
            isUsingGamepad = true;

        float inputX;
        float inputY;

        if (isUsingGamepad)
        {
            inputX = stickDelta.x * controllerSensX * Time.deltaTime;
            inputY = stickDelta.y * controllerSensY * Time.deltaTime;
        }
        else
        {
            inputX = mouseDelta.x * mouseSensX;
            inputY = mouseDelta.y * mouseSensY;
        }

        yRotation += inputX;
        xRotation -= inputY;

        if (clampView)
        {
            xRotation = Mathf.Clamp(xRotation, minX, maxX);
            yRotation = Mathf.Clamp(yRotation, minY, maxY);
        }
        else
        {
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        }

        transform.rotation = Quaternion.Euler(xRotation, yRotation, tiltZ);

        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void SetRotation(float x, float y)
    {
        xRotation = x;
        yRotation = y;
    }

    private void ApplySettings(SettingsData settings)
    {
        mouseSensX = settings.mouseSens;
        mouseSensY = settings.mouseSens;
        controllerSensX = settings.controllerSens;
        controllerSensY = settings.controllerSens;
    }
}
