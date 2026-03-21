using UnityEngine;

public class CrashSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip crash;
    [Range(0f, 1f)]
    public float volume = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        audioSource.PlayOneShot(crash, volume);
    }
}
