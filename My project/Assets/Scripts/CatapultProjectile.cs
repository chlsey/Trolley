using UnityEngine;

public class CatapultProjectile : MonoBehaviour
{
    private Rigidbody rb;
    public float launchForce = 1000f;
    public Vector3 launchDirection = new Vector3(0, 1, 1);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 1. Lock it down initially
        rb.isKinematic = true; 
    }

    public void Launch()
    {
        // 2. "Untie" it from the catapult arm
        transform.SetParent(null); 

        // 3. Turn physics back on
        rb.isKinematic = false;

        // 4. Send it flying!
        rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
    }
}