using UnityEngine;
using UnityEngine.Splines;

public class TrolleyMovement : MonoBehaviour
{
    internal bool switched;
    public float moveSpeed;
    // Vector3 moveDirection;
    Transform orientation;

    public SplineContainer spline;
    public bool followSpline = true;
    private float distanceAlongSpline = 0f;
    
    public LayerMask ground;
    Rigidbody rb;
    
    bool grounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (followSpline && spline != null)
        {
            MoveTrolley();
            return;
        }

    }


    private void MoveTrolley()
    {
        if (spline == null) return;

        float splineLength = spline.GetSplineLength();
        distanceAlongSpline = Mathf.Repeat(distanceAlongSpline, splineLength);
        
        spline.Evaluate(distanceAlongSpline, out Vector3 currentPos, out Vector3 currentTangent);
        
        // get future spline position
        Vector3 desiredDirection = currentTangent.normalized;
        Vector3 desiredVelocity = desiredDirection * moveSpeed;
        
        // calc forward force
        Vector3 velocityDiff = desiredVelocity - rb.linearVelocity;
        rb.AddForce(velocityDiff * rb.mass, ForceMode.Force);
        
        // rotate cube in forward direction
        transform.rotation = Quaternion.LookRotation(currentTangent);
        
        distanceAlongSpline += moveSpeed * Time.deltaTime;
    }
}
