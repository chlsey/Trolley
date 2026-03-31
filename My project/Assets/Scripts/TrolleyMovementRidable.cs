using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class TrolleyMovementRidable : MonoBehaviour
{
    private const float DefaultMoveSpeed = 0.04f;

    internal bool switched;
    public float moveSpeed = DefaultMoveSpeed;

    [SerializeField] private float uphillSlowdown = 2f;
    [SerializeField] private float downhillSpeedup = 0.35f;
    [SerializeField] private float minSlopeSpeedMultiplier = 0.45f;
    [SerializeField] private float maxSlopeSpeedMultiplier = 1.5f;

    public SplineContainer spline1;
    public SplineContainer spline2;
    public SplineContainer currentSpline;
    public bool followSpline = false;
    public AudioSource audioSource;

    [Header("Ride")]
    public Transform seatAnchor;
    public Vector3 riderLocalPosition = new Vector3(0f, 0.35f, 1.65f);
    public Vector3 riderLocalEulerAngles = Vector3.zero;
    public GameObject mountPrompt;
    public float rideMinPitch = -20f;
    public float rideMaxPitch = 20f;
    public float rideMinYaw = -35f;
    public float rideMaxYaw = 35f;

    private float distanceAlongSpline = 0f;
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

        if (moveSpeed <= 0f)
        {
            moveSpeed = DefaultMoveSpeed;
            Debug.LogWarning(
                $"{nameof(TrolleyMovementRidable)} had no move speed configured on '{name}'. Falling back to {DefaultMoveSpeed}.",
                this
            );
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }
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

    private void TryMountPlayer()
    {
        if (isMounted || playerInRange == null)
        {
            return;
        }

        bool gotInput = false;

        if (Input.GetKeyDown(KeyCode.E))
        {
            gotInput = true;
        }
        else if (Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame)
        {
            gotInput = true;
        }

        if (!gotInput)
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

        if (playerMovement == null || playerCamera == null || playerRb == null)
        {
            Debug.LogWarning($"{nameof(TrolleyMovementRidable)} could not mount player. Missing required components on {playerRoot.name}.");
            return;
        }

        Transform rideParent = transform;
        if (seatAnchor != null)
        {
            rideParent = seatAnchor;
        }

        Vector3 playerWorldScale = playerRoot.lossyScale;

        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        playerRoot.SetParent(rideParent, false);
        PreserveWorldScale(playerRoot, playerWorldScale);

        if (seatAnchor != null)
        {
            playerRoot.localPosition = Vector3.zero;
            playerRoot.localRotation = Quaternion.identity;
        }
        else
        {
            playerRoot.localPosition = riderLocalPosition;
            playerRoot.localRotation = Quaternion.Euler(riderLocalEulerAngles);
        }

        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.isKinematic = true;
        playerRb.detectCollisions = false;

        foreach (Collider playerCollider in playerColliders)
        {
            playerCollider.enabled = false;
        }

        playerMovement.platformRb = null;
        playerMovement.enabled = false;

        if (playerMovement.arms != null)
        {
            playerMovement.arms.SetActive(false);
        }

        playerCamera.BeginRideView(rideParent, rideMinPitch, rideMaxPitch, rideMinYaw, rideMaxYaw);

        playerInRange = null;
        isMounted = true;
        followSpline = true;
        ToggleMountPrompt(false);

        string splineName = "None";
        if (currentSpline != null)
        {
            splineName = currentSpline.name;
        }

        Debug.Log(
            $"Player '{playerRoot.name}' mounted trolley '{name}' on spline '{splineName}'.",
            this
        );

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void MoveTrolley()
    {
        if (currentSpline == null)
        {
            return;
        }

        float splineLength = currentSpline.CalculateLength();
        distanceAlongSpline = Mathf.Repeat(distanceAlongSpline, splineLength);

        currentSpline.Evaluate(distanceAlongSpline, out float3 currentPos, out float3 currentTangent, out float3 currentUp);

        transform.position = currentPos;
        transform.rotation = Quaternion.LookRotation(currentTangent, currentUp);

        Vector3 forwardVector = ((Vector3)currentTangent).normalized;
        Vector3 upVector = ((Vector3)currentUp).normalized;
        float slopeSpeedMultiplier = GetSlopeSpeedMultiplier(forwardVector, upVector);

        float splineSpeedMultiplier = 1f;
        if (currentSpline.CompareTag("Loopty"))
        {
            splineSpeedMultiplier = 4.75f;
        }

        float finalSpeed = moveSpeed * splineSpeedMultiplier * slopeSpeedMultiplier;

        distanceAlongSpline += finalSpeed * Time.deltaTime;
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
        switched = !switched;

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

        Rigidbody attached = other.attachedRigidbody;
        GameObject candidate = null;

        if (attached != null)
        {
            candidate = attached.gameObject;
        }
        else
        {
            candidate = other.transform.root.gameObject;
        }

        if (candidate != null && candidate.CompareTag("Player"))
        {
            return candidate.transform;
        }

        return null;
    }

    private void ToggleMountPrompt(bool shouldShow)
    {
        if (mountPrompt == null)
        {
            return;
        }

        bool showPrompt = false;
        if (shouldShow && !isMounted)
        {
            showPrompt = true;
        }

        mountPrompt.SetActive(showPrompt);
    }

    private void PreserveWorldScale(Transform target, Vector3 desiredWorldScale)
    {
        if (target.parent == null)
        {
            target.localScale = desiredWorldScale;
            return;
        }

        Vector3 parentScale = target.parent.lossyScale;

        float localX = desiredWorldScale.x;
        if (parentScale.x != 0f)
        {
            localX = desiredWorldScale.x / parentScale.x;
        }

        float localY = desiredWorldScale.y;
        if (parentScale.y != 0f)
        {
            localY = desiredWorldScale.y / parentScale.y;
        }

        float localZ = desiredWorldScale.z;
        if (parentScale.z != 0f)
        {
            localZ = desiredWorldScale.z / parentScale.z;
        }

        target.localScale = new Vector3(localX, localY, localZ);
    }
}
