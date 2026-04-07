using UnityEngine;

public class MusicBanner : MonoBehaviour
{
    public AudioClip music;
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter()
    {
        audioSource.clip = music;
        audioSource.loop = false;
        audioSource.Play();
        // VOManager.Instance.StartBackgroundMusic(music);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
