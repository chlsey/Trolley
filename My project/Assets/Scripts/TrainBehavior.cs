using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
public class trainBehaviour : MonoBehaviour
{
    public LayerMask layerMask;
    public AudioSource audioSource;
    public AudioClip splat;
    public GameObject ParticlePrefab;
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            audioSource.PlayOneShot(splat);
            UnityEngine.Debug.Log("Touched");
            Instantiate(ParticlePrefab, transform.position, transform.rotation);
        }
    }

    void Update()
    {
        
    }
}


