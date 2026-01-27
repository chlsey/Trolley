using UnityEngine;

public class VictinSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip splat;
    public LayerMask layerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            audioSource.PlayOneShot(splat);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
