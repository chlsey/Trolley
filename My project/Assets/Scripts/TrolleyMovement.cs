using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class TrolleyMovement : MonoBehaviour
{
    internal bool switched;
    public float moveSpeed;
    private float uphillSlowdown = 2f;   
    private float downhillSpeedup = 0.35f;
    private float minSlopeSpeedMultiplier = 0.45f;
    private float maxSlopeSpeedMultiplier = 1.5f;

    public SplineContainer spline1;
    public SplineContainer spline2;
    public SplineContainer currentSpline;
    public bool followSpline = true;
    public AudioSource audioSource;
    public Rigidbody rb;
    public GameObject lever;
    private float distanceAlongSpline = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        currentSpline = spline1;
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

    public void SwitchTrack() {
        if (currentSpline == spline1) {
            currentSpline = spline2;
        }
        else {
            currentSpline = spline1;    
        }
    }

    // used for the huge fan level
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "huge_fan")
        {
            Debug.Log("huge fan hits trolley");
            StartCoroutine(SlowBrakeCoroutine());
        }

    }

    private IEnumerator SlowBrakeCoroutine()
    {
        audioSource.Stop();
        float decelerationRate = 0.5f;

        while (moveSpeed > 0)
        {
            moveSpeed -= decelerationRate * Time.deltaTime;

            // Prevent moveSpeed from going negative
            if (moveSpeed < 0) moveSpeed = 0;

            yield return null; // Wait for the next frame
        }

        Debug.Log("Trolley has come to a complete stop.");
        // yield return null;
    }
}
