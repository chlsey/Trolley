using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public Rigidbody platformRb;
    public float sideAirMultiplier = 0.35f;

    public PlayerCamera cam;
    public GameObject arms;
    public Transform destination;

    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;
    float stepTimer;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float airMultiplier = 0.85f;
    public float oppositeAirMultiplier = 0.4f;

    // ONLY RUNS ON CLUSTER TRUCKS
    public bool useWorldDirectionalAirDrag = false;
    public Vector3 worldDireciton = Vector3.forward;

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
            if (useWorldDirectionalAirDrag)
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
    // Very situational solution
    // Since the trolleys are all moving in one direction, we set a global world direction.
    // Inputs that move the player farther away from the global world direction are induced with MORE air drag
    // This is intended to make jumping feel more consistent between jumping with the trolleys vs jumping against/adjacent
    Vector3 GetDirectionalAirVelocity()
    {
        Vector3 moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 inputDirXZPlane = Vector3.ProjectOnPlane(moveDir, Vector3.up);
        Vector3 trolleyDirXZPlane = Vector3.ProjectOnPlane(worldDireciton, Vector3.up);
        float inputMagnitude = Mathf.Clamp01(inputDirXZPlane.magnitude);


        inputDirXZPlane.Normalize();
        trolleyDirXZPlane.Normalize();

        // calc dot product between planes to get difference in alignment
        float alignment = Vector3.Dot(inputDirXZPlane, trolleyDirXZPlane);

        // remap dot from -1..1 to 0..1 so we can lerp our drag values
        float directionalBlend = (alignment + 1f) * 0.5f;

        // bias the blend harder toward opposite air drag so sideways jumps get dragged more too
        directionalBlend *= directionalBlend;
        directionalBlend *= directionalBlend * directionalBlend;

        // apply air multipler on scale between regular air multipler (moving with trolleys) vs opposite air multipler (moving against trolleys)
        float directionalAirMultiplier = Mathf.Lerp(oppositeAirMultiplier, airMultiplier, directionalBlend);

        // build the players intended air movement from input dir and the scaled multiplier
        Vector3 directionalAirVelocity = inputDirXZPlane * (moveSpeed * directionalAirMultiplier * inputMagnitude);

        // split lastVelocity into world direction vs leftover sideways so we can compare the forward parts cleanly
        Vector3 lastVelocityAlongWorld = Vector3.Project(lastVel, trolleyDirXZPlane);
        Vector3 lastVelocitySideways = lastVel - lastVelocityAlongWorld;

        // do the same split for this frames air input
        Vector3 airAlongWorld = Vector3.Project(directionalAirVelocity, trolleyDirXZPlane);
        Vector3 airSideways = directionalAirVelocity - airAlongWorld;

        // convert the world direction pieces into signed speeds for easier combine logic
        float lastVelocityWorldSpeed = Vector3.Dot(lastVelocityAlongWorld, trolleyDirXZPlane);
        float airWorldSpeed = Vector3.Dot(airAlongWorld, trolleyDirXZPlane);

        if (Mathf.Abs(lastVelocityWorldSpeed) > 0.0001f &&
            Mathf.Abs(airWorldSpeed) > 0.0001f &&
            Mathf.Sign(lastVelocityWorldSpeed) == Mathf.Sign(airWorldSpeed))
        {
            // if both push the same way, keep the stronger one instead of stacking into mach fuck
            airWorldSpeed = Mathf.Sign(airWorldSpeed) * Mathf.Max(Mathf.Abs(lastVelocityWorldSpeed), Mathf.Abs(airWorldSpeed));
        }
        else
        {
            // if they oppose each other, combine them normally so the input can fight the lastVelocity
            airWorldSpeed = lastVelocityWorldSpeed + airWorldSpeed;
        }

        // apply the multiplier and shi to our calced velocities
        return trolleyDirXZPlane * airWorldSpeed + lastVelocitySideways + airSideways;
    }

    void Jump()
    {
        Vector3 currentHorizontalVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        Vector3 groundMoveVelocity = canJump ? GetGroundMoveVelocity() : Vector3.zero;

        // save horizontal velocity on jump
        lastVel = currentHorizontalVelocity - groundMoveVelocity;

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

            // save horizontal velocity on jump
            lastVel = currentHorizontalVelocity - groundMoveVelocity;
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
