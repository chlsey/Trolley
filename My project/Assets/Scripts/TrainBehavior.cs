using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
public class trainBehaviour : MonoBehaviour
{
    public LayerMask layerMask;
    AudioSource audioSource;
    public GameObject ParticlePrefab;
    public float trainSpeed = 2.0f;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            audioSource.Play();
            UnityEngine.Debug.Log("Touched");
            Instantiate(ParticlePrefab);
        }
    }

    void Update()
    {
        // Move the object forward along its z axis 1 unit/second.
        transform.Translate(Vector3.left * Time.deltaTime * trainSpeed);
    }
}


