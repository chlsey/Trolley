using UnityEngine;
using UnityEngine.Splines;

public class TrolleyMovement : MonoBehaviour
{
    internal bool switched;
    public float moveSpeed;
    Vector3 moveDirection;
    Transform orientation;

    [Header("Spline Follow")]
    public bool followSpline = true;
    public SplineContainer spline;
    public bool loopSpline = true;
    [SerializeField] float distanceTravelled;
    
    public LayerMask ground;
    Rigidbody rb;
    
    bool grounded;
    public float playerHeight;
    public float airMultiplier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            Debug.Log("Rigidbody found successfully");
        else
            Debug.Log("Rigidbody NOT found - add one to the cube!");
        
        if (rb != null)
            rb.freezeRotation = !followSpline;
        
        if (orientation == null)
            orientation = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (followSpline && spline != null)
        {
            FollowSpline();
            return;
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        MoveTrolley();
    }

    private void FollowSpline()
    {
        float length = spline.CalculateLength();
        if (length <= 0f)
            return;

        distanceTravelled += moveSpeed * Time.deltaTime;
        float distance = loopSpline
            ? Mathf.Repeat(distanceTravelled, length)
            : Mathf.Clamp(distanceTravelled, 0f, length);
        float t = distance / length;

        if (!spline.Evaluate(t, out var position, out var tangent, out var up))
            return;

        var forward = (Vector3)tangent;
        var rotation = forward.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(forward, (Vector3)up)
            : transform.rotation;

        if (rb != null)
        {
            rb.MovePosition((Vector3)position);
            rb.MoveRotation(rotation);
        }
        else
        {
            transform.SetPositionAndRotation((Vector3)position, rotation);
        }
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
