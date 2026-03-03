using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class TrolleyBehaviour : MonoBehaviour
{
    public LayerMask layerMask;
    public AudioSource audioSource;
    public AudioClip splat;
    public AudioClip applause;
    public GameObject ParticlePrefab;
    public Lever Lever;
    public Rating rating;
    public Transform trolley;
    private Transform prevParent;

    private bool audioPlayed = false;

    public void ResetTrain()
    {
        audioPlayed = false;
    }

    void Awake()
    {
        rating ??= FindFirstObjectByType<Rating>();
        Lever ??= FindFirstObjectByType<Lever>();
    }

     private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log($"[trainBehaviour] OnTriggerEnter: {other.gameObject.name}, layer={other.gameObject.layer}, audioPlayed={audioPlayed}, layerMask={layerMask.value}");
        if (audioPlayed) return;
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            // if (!Lever.leverFlipped)
            // {
            //     switch (Rating.CurrentMode)
            //     {
            //         case RatingMode.Default:
            //             UnityEngine.Debug.Log("Lever not flipped, good job!");
            //             rating.ChangeRating(0.15f);
            //             break;
            //         case RatingMode.AlwaysIncrease:
            //             UnityEngine.Debug.Log("Lever not flipped, audience loves it!");
            //             rating.ChangeRating(0.15f);
            //             break;
            //         case RatingMode.Inverted:
            //             UnityEngine.Debug.Log("Lever not flipped, audience disappointed!");
            //             rating.ChangeRating(-0.10f);
            //             break;
            //     }
            // }
            // else
            // {
            //     switch (Rating.CurrentMode)
            //     {
            //         case RatingMode.Default:
            //             UnityEngine.Debug.Log("Lever flipped, bad job!");
            //             rating.ChangeRating(-0.10f);
            //             break;
            //         case RatingMode.AlwaysIncrease:
            //             UnityEngine.Debug.Log("Lever flipped, audience still loves it!");
            //             rating.ChangeRating(0.15f);
            //             break;
            //         case RatingMode.Inverted:
            //             UnityEngine.Debug.Log("Lever flipped, audience approves!");
            //             rating.ChangeRating(0.15f);
            //             break;
            //     }
            // }
            audioPlayed = true;
            audioSource.PlayOneShot(splat);
            audioSource.PlayOneShot(applause);
            UnityEngine.Debug.Log("Touched");
            Instantiate(ParticlePrefab, transform.position, transform.rotation);
        }
    }
}


