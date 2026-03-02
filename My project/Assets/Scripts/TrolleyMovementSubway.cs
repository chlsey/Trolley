using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

// NOTE: this is for the subway surfer level
public class TrolleyMovementSubway : MonoBehaviour
{

    internal bool switched;
    public float moveSpeed;

    public SplineContainer spline;
    public SplineContainer currentSpline;
    public bool followSpline;
    public AudioSource audioSource;
    public Rigidbody rb;
    public GameObject lever;
    private float distanceAlongSpline = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpline = spline;
        followSpline = false;
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

    void OnTriggerEnter()
    {
        followSpline = true;
    }
    private void MoveTrolley()
    {

        float splineLength = currentSpline.CalculateLength();
        distanceAlongSpline = Mathf.Repeat(distanceAlongSpline, splineLength);

        currentSpline.Evaluate(distanceAlongSpline, out float3 currentPos, out float3 currentTangent, out float3 currentUp);

        // Use MovePosition and MoveRotation instead of transform
        rb.MovePosition(currentPos);
        //Quaternion correction = Quaternion.Euler(0, 90, 0);
        rb.MoveRotation(Quaternion.LookRotation(currentUp));

        if(lever)
        {
            lever.transform.position = currentPos;
        }

        distanceAlongSpline += moveSpeed * Time.deltaTime;
    }

}
