using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CrashSFX : MonoBehaviour
{
    public LayerMask truckLayer;
    public AudioClip crashSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((truckLayer.value & (1 << collision.collider.gameObject.layer)) == 0)
            return;
        audioSource.PlayOneShot(crashSound);
    }
}
