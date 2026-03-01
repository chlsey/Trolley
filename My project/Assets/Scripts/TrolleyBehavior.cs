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
    void Start()
    {
        
    }

    public void ResetTrain()
    {
        audioPlayed = false;
    }

    void Awake()
    {
        rating ??= FindFirstObjectByType<Rating>();
        Lever ??= FindFirstObjectByType<Lever>();
    }
}


