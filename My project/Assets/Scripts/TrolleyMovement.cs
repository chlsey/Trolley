using System.Collections;
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
    public AudioSource audioSource;
    public Rigidbody rb;
    public GameObject lever;
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

    public void SwitchTrack()
    {
        if (currentSpline == spline)
        {
            currentSpline = spline1;
        }
        else
        {
            currentSpline = spline;
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
