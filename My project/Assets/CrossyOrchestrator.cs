using UnityEngine;

public class CrossyOrchestrator : MonoBehaviour
{
    public AudioClip intro;
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (VOManager.Instance != null)
        {
            VOManager.Instance.PlayLine(intro);
        }
        else
        {
            audioSource.clip = intro;
            audioSource.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
