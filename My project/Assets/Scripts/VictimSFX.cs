using UnityEngine;

public class VictimSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip splat;
    public LayerMask layerMask;
    public Animator anim;
    public AudioClip applause;
    // public static int killCount = 0;
    private bool triggered = false;
    private bool coasterPhysicsReleased = false;
    private Rigidbody victimRigidbody;
    // public TextMeshPro text;

    private void Awake()
    {
        victimRigidbody = GetComponent<Rigidbody>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (anim != null)
        {
            anim.speed = Random.Range(0.8f, 1.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || triggered)
        {
            return;
        }

        Debug.Log("Triggered by: " + other.name + " on object: " + gameObject.name);

        if ((layerMask.value & (1 << other.gameObject.layer)) <= 0)
        {
            return;
        }

        TriggerDeath();
    }

    public void TriggerDeath()
    {
        if (triggered)
        {
            return;
        }

        triggered = true;

        if (anim != null)
        {
            anim.SetTrigger("dead");
        }

        KillCounter.AddKill();

        if (audioSource != null)
        {
            if (splat != null)
            {
                audioSource.PlayOneShot(splat);
            }

            if (applause != null)
            {
                audioSource.PlayOneShot(applause);
            }
        }
    }

    public void TriggerCoasterHit(Vector3 hitDirection, float impulse, float upwardImpulse, float torqueImpulse)
    {
        TriggerDeath();

        if (coasterPhysicsReleased || victimRigidbody == null)
        {
            return;
        }

        coasterPhysicsReleased = true;

        Vector3 horizontalDirection = Vector3.ProjectOnPlane(hitDirection, Vector3.up);
        if (horizontalDirection.sqrMagnitude < 0.001f)
        {
            horizontalDirection = transform.forward;
        }

        horizontalDirection.Normalize();

        victimRigidbody.isKinematic = false;
        victimRigidbody.useGravity = true;
        victimRigidbody.WakeUp();
        victimRigidbody.AddForce((horizontalDirection * impulse) + (Vector3.up * upwardImpulse), ForceMode.Impulse);

        Vector3 torqueAxis = Vector3.Cross(Vector3.up, horizontalDirection);
        if (torqueAxis.sqrMagnitude < 0.001f)
        {
            torqueAxis = transform.right;
        }

        victimRigidbody.AddTorque(torqueAxis.normalized * torqueImpulse, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
