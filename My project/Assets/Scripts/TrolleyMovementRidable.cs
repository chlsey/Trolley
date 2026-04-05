using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class TrolleyMovementRidable : MonoBehaviour
{
    public float moveSpeed = 0.04f;

    [SerializeField] private float uphillSlowdown = 2f;
    [SerializeField] private float downhillSpeedup = 0.35f;
    [SerializeField] private float minSlopeSpeedMultiplier = 0.45f;
    [SerializeField] private float maxSlopeSpeedMultiplier = 1.5f;

    public SplineContainer spline1;
    public SplineContainer spline2;
    public SplineContainer currentSpline;
    public bool followSpline = false;

    [Header("Camera")]
    public Transform cameraAnchor;
    public GameObject mountPrompt;
    public float cameraMinPitch = -20f;
    public float cameraMaxPitch = 20f;
    public float cameraMinYaw = -35f;
    public float cameraMaxYaw = 35f;

    private float splineProgress = 0f;
    private Transform playerInRange;
    private bool isMounted;

    private void Awake()
    {
        ToggleMountPrompt(false);
    }

    private void Start()
    {
        currentSpline = spline1;
        followSpline = false;
        // ToggleMountPrompt(false);
    }

    private void OnDisable()
    {
        ToggleMountPrompt(false);
    }

    private void Update()
    {
        if (PauseMenuController.IsPaused)
        {
            return;
        }

        TryMountPlayer();

        if (followSpline && currentSpline != null)
        {
            MoveTrolley();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TryKillVictim(other))
        {
            return;
        }

        if (isMounted)
        {
            return;
        }

        Transform playerRoot = GetPlayerRoot(other);
        if (playerRoot == null)
        {
            return;
        }

        playerInRange = playerRoot;
        ToggleMountPrompt(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isMounted)
        {
            return;
        }

        Transform playerRoot = GetPlayerRoot(other);
        if (playerRoot == null || playerRoot != playerInRange)
        {
            return;
        }

        playerInRange = null;
        ToggleMountPrompt(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        TryKillVictim(collision.collider);
    }

    private void TryMountPlayer()
    {
        if (isMounted || playerInRange == null)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.E) &&
            (Gamepad.current == null || !Gamepad.current.buttonWest.wasReleasedThisFrame))
        {
            return;
        }

        MountPlayer(playerInRange);
    }

    private void MountPlayer(Transform playerRoot)
    {
        PlayerMovement playerMovement = playerRoot.GetComponent<PlayerMovement>();
        PlayerCamera playerCamera = playerRoot.GetComponentInChildren<PlayerCamera>();
        Rigidbody playerRb = playerRoot.GetComponent<Rigidbody>();
        Collider[] playerColliders = playerRoot.GetComponentsInChildren<Collider>(true);
        Renderer[] playerRenderers = playerRoot.GetComponentsInChildren<Renderer>(true);

        if (playerMovement == null || playerCamera == null || playerRb == null)
        {
            Debug.LogWarning($"{nameof(TrolleyMovementRidable)} could not mount player. Missing required components on {playerRoot.name}.");
            return;
        }

        if (cameraAnchor == null)
        {
            Debug.LogWarning($"{nameof(TrolleyMovementRidable)} on '{name}' is missing a camera anchor.", this);
            return;
        }

        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.isKinematic = true;
        playerRb.detectCollisions = false;

        foreach (Collider playerCollider in playerColliders)
        {
            playerCollider.enabled = false;
        }

        foreach (Renderer playerRenderer in playerRenderers)
        {
            playerRenderer.enabled = false;
        }

        playerMovement.platformRb = null;
        playerMovement.enabled = false;

        if (playerMovement.arms != null)
        {
            playerMovement.arms.SetActive(false);
        }

        playerCamera.BeginRideView(cameraAnchor, cameraMinPitch, cameraMaxPitch, cameraMinYaw, cameraMaxYaw);

        playerInRange = null;
        isMounted = true;
        followSpline = true;
        ToggleMountPrompt(false);

        // Trigger riding voice line after a short delay
        CoasterEndingController coasterController = FindObjectOfType<CoasterEndingController>();
        if (coasterController != null)
        {
            coasterController.StartRidingCoroutine();
        }

        Debug.Log(
            $"Player '{playerRoot.name}' mounted trolley '{name}' on spline '{(currentSpline != null ? currentSpline.name : "None")}'.",
            this
        );
    }

    private void MoveTrolley()
    {
        if (currentSpline == null)
        {
            return;
        }

        float normalizedProgress = Mathf.Clamp01(splineProgress);

        currentSpline.Evaluate(normalizedProgress, out float3 currentPos, out float3 currentTangent, out float3 currentUp);

        transform.position = currentPos;
        transform.rotation = Quaternion.LookRotation(currentTangent, currentUp);

        Vector3 forwardVector = ((Vector3)currentTangent).normalized;
        Vector3 upVector = ((Vector3)currentUp).normalized;
        float slopeSpeedMultiplier = GetSlopeSpeedMultiplier(forwardVector, upVector);

        float splineSpeedMultiplier = currentSpline.CompareTag("Loopty") ? 4.75f : 1f;
        float finalSpeed = moveSpeed * splineSpeedMultiplier * slopeSpeedMultiplier;

        splineProgress = Mathf.Min(1f, splineProgress + finalSpeed * Time.deltaTime);
        if (splineProgress >= 1f)
        {
            Debug.Log("Spline progress at full 100");
            currentSpline.Evaluate(1f, out currentPos, out currentTangent, out currentUp);
            transform.position = currentPos;
            transform.rotation = Quaternion.LookRotation(currentTangent, currentUp);
            followSpline = false;
            NextScene nextScene = FindObjectOfType<NextScene>();
            if (nextScene != null)
            {
                Debug.Log("next scene loading");
                nextScene.TriggerSceneChange();
            }
        }
    }

    private float GetSlopeSpeedMultiplier(Vector3 forwardVector, Vector3 upVector)
    {
        Vector3 gravityAlongTrack = Vector3.ProjectOnPlane(Physics.gravity.normalized, upVector);
        float slopeDirection = Vector3.Dot(forwardVector, gravityAlongTrack);

        float slopeMultiplier = 1f;
        if (slopeDirection < 0f)
        {
            slopeMultiplier += slopeDirection * uphillSlowdown;
        }
        else if (slopeDirection > 0f)
        {
            slopeMultiplier += slopeDirection * downhillSpeedup;
        }

        return Mathf.Clamp(slopeMultiplier, minSlopeSpeedMultiplier, maxSlopeSpeedMultiplier);
    }

    public void SwitchTrack()
    {
        if (currentSpline == spline1 && spline2 != null)
        {
            currentSpline = spline2;
        }
        else if (spline1 != null)
        {
            currentSpline = spline1;
        }
    }

    private Transform GetPlayerRoot(Collider other)
    {
        if (other == null)
        {
            return null;
        }

        GameObject candidate = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.transform.root.gameObject;

        if (candidate != null && candidate.CompareTag("Player"))
        {
            return candidate.transform;
        }

        return null;
    }

    private bool TryKillVictim(Collider other)
    {
        if (!followSpline || other == null)
        {
            return false;
        }

        VictimSFX victim = other.GetComponentInParent<VictimSFX>();
        if (victim == null)
        {
            return false;
        }

        victim.TriggerDeath();
        return true;
    }

    private void ToggleMountPrompt(bool shouldShow)
    {
        if (mountPrompt == null)
        {
            return;
        }

        mountPrompt.SetActive(shouldShow && !isMounted);
    }
}
