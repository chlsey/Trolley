using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class TrolleyMovement : MonoBehaviour
{
    
    internal bool switched;
    public float moveSpeed;

    public SplineContainer spline;
    public SplineContainer spline1;
    public SplineContainer currentSpline;
    public bool followSpline = true;
    private float distanceAlongSpline = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        currentSpline = spline;
    }

    // Update is called once per frame
    void Update()
    {
        if (followSpline && currentSpline != null)
        {
            MoveTrolley();
            return;
        }
    }


    private void MoveTrolley()
    {
        if (currentSpline == null) return;

        float splineLength = currentSpline.CalculateLength();
        distanceAlongSpline = Mathf.Repeat(distanceAlongSpline, splineLength);
        
        currentSpline.Evaluate(distanceAlongSpline, out float3 currentPos, out float3 currentTangent, out float3 currentUp);
        
        // Set transform position directly along spline
        transform.position = currentPos;
        
        // Set rotation to face forward direction
        transform.rotation = Quaternion.LookRotation(currentTangent);
        
        distanceAlongSpline += moveSpeed * Time.deltaTime;
    }

    public void SwitchTrack() {
        if (currentSpline == spline) {
            currentSpline = spline1;
        }
        else {
            currentSpline = spline;    
        }
    }
}
