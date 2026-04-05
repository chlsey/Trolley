using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public Rigidbody platformRb;

    public PlayerCamera cam;
    public GameObject arms;
    public Transform destination;

    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;
    float stepTimer;

    [Header("Movement")]
    public float moveSpeed = 8f;

    // determines how much we can accelerate in the forward vector while in air
    // -> affects forward/back correction
    public float airMultiplier = 0.2f;

    // determines how fast we can accelerate in the right vector while in air
    // -> affects side to side correction
    public float sideAirMultiplier = 0.9f;

    // determines the players horizontal velocity at initial jump
    public float horziontalJumpMultiplier = 0.5f;

    // determines the max correction velocity
    // -> THIS IS a % OF THE INITAL JUMP VELOCITY
    public float maxCorrectionMultiplier = 0.3f;
    public bool useClusterTruckJump = false;

    // CLUSTER TRUCK JUMP CALL CHAIN
    // INITIAL JUMP -> Jump() -> lastVel/airCorrectionVel setup -> MovePlayer()
    // WHILE IN AIR -> MovePlayer() -> GetDirectionalAirVelocity() -> baseAirMomentum + airCorrectionVel

    [Header("Jump")]
    public float jumpForce = 7f;
    public float jumpCooldown = 0.25f;
    bool readyToJump;
    public bool canJump;

    [Header("Ground Layers")]
    public LayerMask whatIsGround;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 lastVel;
    Vector3 airCorrectionVel;

    Rigidbody rb;

    public KeyCode jumpKey = KeyCode.Space;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        readyToJump = true;
        stepTimer = stepInterval;
    }

    void Update()
    {
        if (PauseMenuController.IsPaused) return;

        MyInput();
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if ((Input.GetKey(jumpKey) ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed))
            && readyToJump && canJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 targetVel;

        if (canJump)
        {
            targetVel = GetGroundMoveVelocity();
        }
        else
        {
            if (useClusterTruckJump)
            {
                targetVel = GetDirectionalAirVelocity();
            }
            else
            {
                targetVel = GetRegularAirVelocity();
            }
        }

        Vector3 velocityChange = new Vector3(
            targetVel.x - velocity.x,
            0,
            targetVel.z - velocity.z
        );

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        if (canJump && platformRb != null)
        {
            Vector3 pv = platformRb.linearVelocity;
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x + pv.x,
                rb.linearVelocity.y,
                rb.linearVelocity.z + pv.z
            );
        }
    }

    Vector3 GetGroundMoveVelocity()
    {
        Vector3 moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDir.Normalize();
        return moveDir * moveSpeed;
    }

    // This is the regular air velocity system (the old version) used for non cluster truck levels
    Vector3 GetRegularAirVelocity()
    {
        Vector3 forwardAirVel = orientation.forward * (verticalInput * moveSpeed * airMultiplier);
        Vector3 sidewaysAirVel = orientation.right * (horizontalInput * moveSpeed * sideAirMultiplier);
        return forwardAirVel + sidewaysAirVel;
    }

    // This is the cluster trucks air velocity calculation
    // We keep the takeoff speed locked while airborne, with tiny accumulated input correction
    Vector3 GetDirectionalAirVelocity()
    {
        Vector3 baseAirMomentum = Vector3.ProjectOnPlane(lastVel, Vector3.up);
        float baseAirSpeed = baseAirMomentum.magnitude;

        if (baseAirSpeed < 0.0001f)
        {
            return baseAirMomentum;
        }

        Vector3 airInput = orientation.forward * (verticalInput * baseAirSpeed * airMultiplier * Time.fixedDeltaTime) +
            orientation.right * (horizontalInput * baseAirSpeed * sideAirMultiplier * Time.fixedDeltaTime);
        airInput = Vector3.ProjectOnPlane(airInput, Vector3.up);

        if (airInput.sqrMagnitude > 0.0001f)
        {
            airCorrectionVel += airInput;

            float maxCorrectionSpeed = baseAirSpeed * maxCorrectionMultiplier;
            if (airCorrectionVel.magnitude > maxCorrectionSpeed)
            {
                airCorrectionVel = airCorrectionVel.normalized * maxCorrectionSpeed;
            }
        }

        return baseAirMomentum + airCorrectionVel;
    }
    
    // NOTE:
    // REMOVED OLD INPUT BASED AIR SPEED
    // MAJORITY OF AIR VELOCITY IS NOW DETEMRINED AT INITAL JUMP -> MOMENTUM BASED JUMPING
    // PLAYER CAN MINORLY AFFECT AIR VELOCITY IN-FLIGHT WITH INPUTS
    void Jump()
    {
        Vector3 currentHorizontalVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        Vector3 groundMoveVelocity = canJump ? GetGroundMoveVelocity() : Vector3.zero;

        // save horizontal velocity on jump
        if (platformRb != null)
        {
            lastVel = Vector3.ProjectOnPlane(platformRb.linearVelocity, Vector3.up) + groundMoveVelocity;
        }
        else
        {
            lastVel = currentHorizontalVelocity;
        }

        if (useClusterTruckJump)
        {
            lastVel *= maxCorrectionMultiplier;
        }

        airCorrectionVel = Vector3.zero;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsGround) != 0)
        {
            canJump = false; 

            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    canJump = true;
                    platformRb = collision.rigidbody;
                    airCorrectionVel = Vector3.zero;
                    return;
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsGround) != 0)
        {
            Vector3 currentHorizontalVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
            Vector3 groundMoveVelocity = canJump ? GetGroundMoveVelocity() : Vector3.zero;

            // save horizontal velocity when leaving the ground
            if (platformRb != null)
            {
                lastVel = Vector3.ProjectOnPlane(platformRb.linearVelocity, Vector3.up) + groundMoveVelocity;
            }
            else
            {
                lastVel = currentHorizontalVelocity;
            }
            airCorrectionVel = Vector3.zero;
            canJump = false;

            if (collision.rigidbody == platformRb)
                platformRb = null;
        }
    }

    void HandleFootsteps()
    {
        if (canJump && (Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0))
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = stepInterval;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0 || footstepSource == null) return;

        int index = Random.Range(0, footstepClips.Length);
        footstepSource.PlayOneShot(footstepClips[index]);
    }

    public void TeleportToTrack()
    {
        cam.tiltZ = -90f;

        transform.rotation = Quaternion.Euler(-90f, 180f, 0f);

        cam.SetRotation(-90f, -90f);

        cam.clampView = true;
        cam.minX = -20f;
        cam.maxX = 20f;
        cam.minY = cam.yRotation - 25f;
        cam.maxY = cam.yRotation + 25f;

        arms.SetActive(false);

        transform.position = destination.position;

        GetComponent<PlayerMovement>().enabled = false;
    }
}
