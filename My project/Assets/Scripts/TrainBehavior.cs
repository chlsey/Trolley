using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
public class trainBehaviour : MonoBehaviour
{
    public LayerMask layerMask;
    public AudioSource audioSource;
    public AudioClip splat;
    public AudioClip applause;
    public GameObject ParticlePrefab;
    public Lever Lever;
    public Rating rating;

    private bool audioPlayed = false;
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (audioPlayed) return;
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            if (!Lever.leverFlipped)
            {
                UnityEngine.Debug.Log("Lever not flipped, good job!");
                rating.ChangeRating(0.15f);
            }

            if (Lever.leverFlipped)
            {
                UnityEngine.Debug.Log("Lever flipped, bad job!");
                rating.ChangeRating(-0.10f);
            }
            audioPlayed = true;
            audioSource.PlayOneShot(splat);
            audioSource.PlayOneShot(applause);
            UnityEngine.Debug.Log("Touched");
            Instantiate(ParticlePrefab, transform.position, transform.rotation);
        }
    }

    
    void Awake()
    {
        rating ??= FindFirstObjectByType<Rating>();
        Lever ??= FindFirstObjectByType<Lever>();
    }
}


