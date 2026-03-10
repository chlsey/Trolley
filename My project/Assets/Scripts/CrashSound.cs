using UnityEngine;

public class CrashSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip crash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        audioSource.PlayOneShot(crash);
    }
}
