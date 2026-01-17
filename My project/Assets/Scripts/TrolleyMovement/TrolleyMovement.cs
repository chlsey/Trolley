using UnityEngine;
using UnityEngine.Splines;

public class TrolleyMovement : MonoBehaviour
{
    internal bool switched;
    public float moveSpeed;
    // Vector3 moveDirection;
    Transform orientation;

    public SplineContainer OriginalTrack;
    public SplineContainer SwitchedTrack;

    SplineContainer CurrentSpline; 

    public bool followSpline = true;
    private float distanceAlongSpline = 0f;
    
    public LayerMask ground;
    Rigidbody rb;
    
    bool grounded;

    void Start()
    {   
        rb = GetComponent<Rigidbody>();

    }

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
        if (CurrentSpline == null) return;

        float splineLength = CurrentSpline.GetSplineLength();
        distanceAlongSpline = Mathf.Repeat(distanceAlongSpline, splineLength);
        
        CurrentSpline.Evaluate(distanceAlongSpline, out Vector3 currentPos, out Vector3 currentTangent);
        
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
