using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    public float xRotation;
    public float yRotation;
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
    }
    private void Update()
    {
        // Get input values from both devices
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        Vector2 stickDelta = Gamepad.current.rightStick.ReadValue();

        // Detect which device has movement (mouse gets priority)
        if (mouseDelta.sqrMagnitude > 0.1f) 
        {
            isUsingGamepad = false;
        }
        else if (stickDelta.sqrMagnitude > 0.1f)
        {
            isUsingGamepad = true;
        }

        // Apply the  input
        float inputX, inputY;

        if (isUsingGamepad)
        {
            // Gamepads usually need Time.deltaTime for consistent rotation speed
            inputX = stickDelta.x * sensX * Time.deltaTime;
            inputY = stickDelta.y * sensY * Time.deltaTime;
        }
        else
        {
            // Mouse Delta is already frame-rate independent in the new Input System
            inputX = mouseDelta.x * sensX * 0.1f; 
            inputY = mouseDelta.y * sensY * 0.1f;
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

        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    public void SetRotation(float x, float y)
    {
        xRotation = x;
        yRotation = y;
    }

}