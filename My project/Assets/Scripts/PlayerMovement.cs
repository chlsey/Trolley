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
    public float airMultiplier = 0.6f;

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
            Vector3 moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            moveDir.Normalize();
            targetVel = moveDir * moveSpeed;
        }
        else
        {
            // calculate forward
            Vector3 forwardAirVel = orientation.forward * (verticalInput * moveSpeed * airMultiplier);
            Vector3 sidewaysAirVel = orientation.right * (horizontalInput * moveSpeed * sideAirMultiplier);
            targetVel = forwardAirVel + sidewaysAirVel;
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

    void Jump()
    {
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
