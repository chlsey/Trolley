using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public Rigidbody platformRb;
    public PlayerCamera cam;
    public GameObject arms;
    public Transform destination;
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;
    private float stepTimer;
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    public KeyCode jumpKey = KeyCode.Space;
    Vector3 moveDirection;
    Rigidbody rb;


    private void Start()
    {
        readyToJump = true;
        stepTimer = stepInterval;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();

        SpeedControl();
        rb.linearDamping = grounded ? groundDrag : 0;
        HandleFootsteps();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if ((Input.GetKey(jumpKey) || (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed)) && readyToJump)
        {
            readyToJump = false;
            Jump();
            Debug.Log("jumped!");
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }
    private void SpeedControl()
    {
    if (platformRb != null)
        {
            // Get the platform's current velocity
            Vector3 platformVel = platformRb.linearVelocity;

            // Calculate the player's horizontal velocity relative to the world
            Vector3 currentHorizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            Vector3 platformHorizontalVel = new Vector3(platformVel.x, 0, platformVel.z);

            // This is the magic part: 
            // We set the velocity to be (Platform Speed) + (Player's own walking speed)
            // We subtract the platform speed from our current speed to see how fast WE are walking
            Vector3 playerWalkingVel = currentHorizontalVel - platformHorizontalVel;

            rb.linearVelocity = new Vector3(platformHorizontalVel.x, rb.linearVelocity.y, platformHorizontalVel.z) + playerWalkingVel;
        }

    }
    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void HandleFootsteps()
    {
        if (grounded && (Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0))
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
    private void PlayFootstep()
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
        gameObject.transform.position = destination.transform.position;


        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.enabled = false;
    }
}