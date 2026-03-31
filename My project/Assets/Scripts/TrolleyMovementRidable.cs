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
    public Rigidbody rb;
    public GameObject lever;

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
    private Transform mountedPlayer;
    private PlayerMovement mountedPlayerMovement;
    private PlayerCamera mountedPlayerCamera;
    private Rigidbody mountedPlayerRb;
    private CapsuleCollider mountedPlayerCapsule;
    private Collider[] mountedPlayerColliders;
    private bool isMounted;

    private void Reset()
    {
        ResolveReferences();
    }

    private void Awake()
    {
        ResolveReferences();
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
            audioSource.Stop();
    }

    private void OnDisable()
    {
        ToggleMountPrompt(false);
    }

    private void Update()
    {
        if (PauseMenuController.IsPaused)
            return;

        TryMountPlayer();

        if (followSpline && currentSpline != null)
            MoveTrolley();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMounted)
            return;

        Transform playerRoot = GetPlayerRoot(other);
        if (playerRoot == null)
            return;

        playerInRange = playerRoot;
        ToggleMountPrompt(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isMounted)
            return;

        Transform playerRoot = GetPlayerRoot(other);
        if (playerRoot == null || playerRoot != playerInRange)
            return;

        playerInRange = null;
        ToggleMountPrompt(false);
    }

    private void TryMountPlayer()
    {
        if (isMounted || playerInRange == null)
            return;

        bool gotInput = Input.GetKeyDown(KeyCode.E) ||
            (Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame);

        if (!gotInput)
            return;

        MountPlayer(playerInRange);
        // StartRide(playerInRange);
    }

    private void StartRide(Transform playerRoot)
    {
        ResolveReferences();

        if (playerRoot != null)
        {
            Rigidbody playerRb = playerRoot.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }

            IgnorePlayerCollisions(playerRoot);
        }

        playerInRange = null;
        isMounted = true;
        followSpline = true;
        ToggleMountPrompt(false);

        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    private void MountPlayer(Transform playerRoot)
    {
        ResolveReferences();

        PlayerMovement playerMovement = playerRoot.GetComponent<PlayerMovement>();
        PlayerCamera playerCamera = playerRoot.GetComponentInChildren<PlayerCamera>();
        Rigidbody playerRb = playerRoot.GetComponent<Rigidbody>();
        CapsuleCollider playerCapsule = playerRoot.GetComponentInChildren<CapsuleCollider>();
        Collider[] playerColliders = playerRoot.GetComponentsInChildren<Collider>(true);

        if (playerMovement == null || playerCamera == null || playerRb == null)
        {
            Debug.LogWarning($"{nameof(TrolleyMovementRidable)} could not mount player. Missing required components on {playerRoot.name}.");
            return;
        }

        Transform rideParent = seatAnchor != null ? seatAnchor : transform;
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
            playerCollider.enabled = false;

        playerMovement.platformRb = null;
        playerMovement.enabled = false;

        if (playerMovement.arms != null)
            playerMovement.arms.SetActive(false);

        playerCamera.BeginRideView(rideParent, rideMinPitch, rideMaxPitch, rideMinYaw, rideMaxYaw);

        mountedPlayer = playerRoot;
        mountedPlayerMovement = playerMovement;
        mountedPlayerCamera = playerCamera;
        mountedPlayerRb = playerRb;
        mountedPlayerCapsule = playerCapsule;
        mountedPlayerColliders = playerColliders;
        playerInRange = null;
        isMounted = true;
        followSpline = true;
        ToggleMountPrompt(false);

        Debug.Log(
            $"Player '{playerRoot.name}' mounted trolley '{name}' on spline '{currentSpline?.name ?? "None"}'.",
            this
        );

        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    private void MoveTrolley()
    {
        
        if (currentSpline == null) return;

        float splineLength = currentSpline.CalculateLength();
        distanceAlongSpline = Mathf.Repeat(distanceAlongSpline, splineLength);

        currentSpline.Evaluate(distanceAlongSpline, out float3 currentPos, out float3 currentTangent, out float3 currentUp);
        
        // NOTE: only for subway surfers
        if(rb) {
            // move the trolley's RB
            rb.MovePosition(currentPos);
            // // Set rotation to face forward direction
            rb.MoveRotation(Quaternion.LookRotation(currentUp));
        }
        else
        {
            // Set transform position directly along spline
            transform.position = currentPos;
        
            // Follow the spline's forward and banking in non-Rigidbody mode.
            transform.rotation = Quaternion.LookRotation(currentTangent, currentUp);
        }

        Vector3 forwardVector = ((Vector3)currentTangent).normalized;
        Vector3 upVector = ((Vector3)currentUp).normalized;
        float slopeSpeedMultiplier = GetSlopeSpeedMultiplier(forwardVector, upVector);
        float splineSpeedMultiplier = currentSpline.CompareTag("Loopty") ? 4.75f : 1f;
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

    private void ResolveReferences()
    {
        UnityEngine.SceneManagement.Scene currentScene = gameObject.scene;
        TrolleyMovement legacyMovement = GetComponent<TrolleyMovement>();

        if (legacyMovement != null)
        {
            if (moveSpeed <= 0f)
                moveSpeed = legacyMovement.moveSpeed;

            spline1 ??= legacyMovement.spline1;
            spline2 ??= legacyMovement.spline2;
            currentSpline ??= legacyMovement.currentSpline;
            audioSource ??= legacyMovement.audioSource;
            rb ??= legacyMovement.rb;
            lever ??= legacyMovement.lever;
        }

        rb ??= GetComponent<Rigidbody>();
        audioSource ??= GetComponent<AudioSource>();

        if (spline1 == null || spline2 == null)
        {
            SplineContainer[] splineContainers = Resources.FindObjectsOfTypeAll<SplineContainer>();

            foreach (SplineContainer splineContainer in splineContainers)
            {
                if (!IsSceneObject(splineContainer.gameObject, currentScene))
                    continue;

                if (spline1 == null && splineContainer.gameObject.name == "OrgPath")
                    spline1 = splineContainer;

                if (spline2 == null && splineContainer.gameObject.name == "SwitchedPath")
                    spline2 = splineContainer;
            }
        }

        if (mountPrompt == null)
        {
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();

            foreach (Transform candidate in allTransforms)
            {
                if (!IsSceneObject(candidate.gameObject, currentScene))
                    continue;

                if (candidate.name == "MountPrompt")
                {
                    mountPrompt = candidate.gameObject;
                    break;
                }
            }
        }
    }

    private Transform GetPlayerRoot(Collider other)
    {
        if (other == null)
            return null;

        Rigidbody attached = other.attachedRigidbody;
        GameObject candidate = attached != null ? attached.gameObject : other.transform.root.gameObject;

        if (candidate != null && candidate.CompareTag("Player"))
            return candidate.transform;

        return null;
    }

    private void ToggleMountPrompt(bool shouldShow)
    {
        if (mountPrompt == null)
            return;

        mountPrompt.SetActive(shouldShow && !isMounted);
    }

    private void PreserveWorldScale(Transform target, Vector3 desiredWorldScale)
    {
        if (target.parent == null)
        {
            target.localScale = desiredWorldScale;
            return;
        }

        Vector3 parentScale = target.parent.lossyScale;

        target.localScale = new Vector3(
            parentScale.x == 0f ? desiredWorldScale.x : desiredWorldScale.x / parentScale.x,
            parentScale.y == 0f ? desiredWorldScale.y : desiredWorldScale.y / parentScale.y,
            parentScale.z == 0f ? desiredWorldScale.z : desiredWorldScale.z / parentScale.z
        );
    }

    private void IgnorePlayerCollisions(Transform playerRoot)
    {
        Collider[] playerColliders = playerRoot.GetComponentsInChildren<Collider>(true);
        Collider[] trolleyColliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider playerCollider in playerColliders)
        {
            if (playerCollider == null)
                continue;

            foreach (Collider trolleyCollider in trolleyColliders)
            {
                if (trolleyCollider == null || trolleyCollider == playerCollider)
                    continue;

                Physics.IgnoreCollision(playerCollider, trolleyCollider, true);
            }
        }
    }

    private bool IsSceneObject(GameObject gameObject, UnityEngine.SceneManagement.Scene expectedScene)
    {
        return gameObject.scene.IsValid() && gameObject.scene.isLoaded && gameObject.scene == expectedScene;
    }
}
