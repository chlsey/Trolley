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
    // public TextMeshPro text;

    private void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
