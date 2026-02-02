using UnityEngine;

public class VOManager : MonoBehaviour
{
    public static VOManager Instance;
    public AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    public void Play(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(clip);
    }
}
