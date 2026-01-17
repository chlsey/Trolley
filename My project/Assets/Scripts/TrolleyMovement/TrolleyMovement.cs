using UnityEngine;

public class TrolleyMovement : MonoBehaviour
{
    internal bool switched;
    public float moveSpeed;
    Vector3 moveDirection;
    public Transform orientation;
    
    public LayerMask ground;
    Rigidbody rb;
    
    bool grounded;
    public float playerHeight;
    public float airMultiplier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MoveTrolley();
    }

    private void MoveTrolley()
    {
        moveDirection = orientation.forward;
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
}
