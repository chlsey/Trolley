using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ClusterTruckMovement : MonoBehaviour
{
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public float acceleration = 10f;
    public float changeInterval = 1.5f;

    private float targetSpeed;
    private float currentSpeed;
    private float timer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        currentSpeed = minSpeed;
        PickNewTargetSpeed();
    }

    void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;

        if (timer <= 0f)
            PickNewTargetSpeed();
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        Vector3 desired = transform.forward * currentSpeed;
        Vector3 change = desired - rb.linearVelocity;
        change.y = 0;
        rb.AddForce(change, ForceMode.VelocityChange);
    }

    void PickNewTargetSpeed()
    {
        targetSpeed = Random.Range(minSpeed, maxSpeed);
        timer = changeInterval;
    }
}
