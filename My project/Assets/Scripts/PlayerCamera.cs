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
    public Transform cameraRigRoot;

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
    private bool rideViewActive = false;
    private Transform rideReference;
    private float rideXRotation;
    private float rideYRotation;
    private float rideMinX = -20f;
    private float rideMaxX = 20f;
    private float rideMinY = -35f;
    private float rideMaxY = 35f;

    private bool savedClampView;
    private float savedMinX;
    private float savedMaxX;
    private float savedMinY;
    private float savedMaxY;
    private float savedTiltZ;
    private Transform savedCameraRigParent;
    private Vector3 savedCameraRigLocalPosition;
    private Quaternion savedCameraRigLocalRotation;
    private Vector3 savedCameraRigLocalScale;

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

        if (rideViewActive)
        {
            rideYRotation += inputX;
            rideXRotation -= inputY;

            rideXRotation = Mathf.Clamp(rideXRotation, rideMinX, rideMaxX);
            rideYRotation = Mathf.Clamp(rideYRotation, rideMinY, rideMaxY);

            if (rideReference != null)
            {
                transform.rotation = rideReference.rotation * Quaternion.Euler(rideXRotation, rideYRotation, 0f);

                if (orientation != null)
                {
                    float baseYaw = rideReference.eulerAngles.y;
                    orientation.rotation = Quaternion.Euler(0f, baseYaw + rideYRotation, 0f);
                }
            }

            return;
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

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, tiltZ);

        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void SetRotation(float x, float y)
    {
        xRotation = x;
        yRotation = y;
    }

    public void BeginRideView(Transform reference, float minRideX, float maxRideX, float minRideY, float maxRideY)
    {
        if (reference == null)
            return;

        Transform rigRoot = ResolveCameraRigRoot();
        if (rigRoot == null)
        {
            Debug.LogWarning($"{nameof(PlayerCamera)} on '{name}' is missing a camera rig root.", this);
            return;
        }

        savedClampView = clampView;
        savedMinX = minX;
        savedMaxX = maxX;
        savedMinY = minY;
        savedMaxY = maxY;
        savedTiltZ = tiltZ;
        savedCameraRigParent = rigRoot.parent;
        savedCameraRigLocalPosition = rigRoot.localPosition;
        savedCameraRigLocalRotation = rigRoot.localRotation;
        savedCameraRigLocalScale = rigRoot.localScale;

        rideReference = reference;
        rideMinX = minRideX;
        rideMaxX = maxRideX;
        rideMinY = minRideY;
        rideMaxY = maxRideY;
        rideXRotation = 0f;
        rideYRotation = 0f;
        rideViewActive = true;
        tiltZ = 0f;

        SetParentPreservingWorldScale(rigRoot, rideReference);
        rigRoot.localPosition = new Vector3(0.5f,0.25f,0.310000002f);
        rigRoot.localRotation = Quaternion.identity;

        transform.rotation = rideReference.rotation;

        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0f, rideReference.eulerAngles.y, 0f);
    }

    public void ClearRideView()
    {
        rideViewActive = false;
        rideReference = null;
        rideXRotation = 0f;
        rideYRotation = 0f;

        clampView = savedClampView;
        minX = savedMinX;
        maxX = savedMaxX;
        minY = savedMinY;
        maxY = savedMaxY;
        tiltZ = savedTiltZ;

        Transform rigRoot = ResolveCameraRigRoot();
        if (rigRoot != null)
        {
            rigRoot.SetParent(savedCameraRigParent, false);
            rigRoot.localPosition = savedCameraRigLocalPosition;
            rigRoot.localRotation = savedCameraRigLocalRotation;
            rigRoot.localScale = savedCameraRigLocalScale;
        }

        Vector3 euler = transform.rotation.eulerAngles;
        xRotation = NormalizeAngle(euler.x);
        yRotation = NormalizeAngle(euler.y);
    }

    private void ApplySettings(SettingsData settings)
    {
        mouseSensX = settings.mouseSens;
        mouseSensY = settings.mouseSens;
        controllerSensX = settings.controllerSens;
        controllerSensY = settings.controllerSens;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private Transform ResolveCameraRigRoot()
    {
        if (cameraRigRoot != null)
            return cameraRigRoot;

        Transform cameraHolder = transform.parent;
        if (cameraHolder != null)
            return cameraHolder.parent;

        return null;
    }

    private void SetParentPreservingWorldScale(Transform child, Transform newParent)
    {
        Vector3 worldScale = child.lossyScale;

        child.SetParent(newParent, false);
        child.localScale = DivideVectorComponents(worldScale, newParent != null ? newParent.lossyScale : Vector3.one);
    }

    private Vector3 DivideVectorComponents(Vector3 numerator, Vector3 denominator)
    {
        return new Vector3(
            DivideOrFallback(numerator.x, denominator.x),
            DivideOrFallback(numerator.y, denominator.y),
            DivideOrFallback(numerator.z, denominator.z)
        );
    }

    private float DivideOrFallback(float numerator, float denominator)
    {
        if (Mathf.Approximately(denominator, 0f))
            return numerator;

        return numerator / denominator;
    }
}
